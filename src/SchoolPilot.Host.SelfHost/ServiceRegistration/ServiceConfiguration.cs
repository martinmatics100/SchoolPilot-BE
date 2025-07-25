// SchoolPilot.Host.SelfHost.ServiceRegistration.ServiceConfiguration.cs
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Api.Filters;
using System.Reflection;
using SchoolPilot.Host.DIContainer.Modules;
using SchoolPilot.Data.Context;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Data.Helpers;
using SchoolPilot.Business.Interfaces;
using SchoolPilot.Infrastructure.Commands.Assets;
using Refit;
using SchoolPilot.Infrastructure.Validators;
using StackExchange.Redis;
using SchoolPilot.Infrastructure.BasicResults;
using Microsoft.AspNetCore.Mvc.Controllers;
using SchoolPilot.Common.Interfaces;
using SchoolPilot.Host.Hangfire;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.System.Text.Json;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using SchoolPilot.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer; // NEW
using Microsoft.IdentityModel.Tokens; // NEW
using System.Text;
using SchoolPilot.Api.Controllers;
using SchoolPilot.Api.Controllers.StandAlone;
using Supabase;
using SchoolPilot.Infrastructure.Helpers.SuperbaseHelper;
using SchoolPilot.Common.Constants;
using SchoolPilot.Data.Entities.lookup; // NEW

namespace SchoolPilot.Host.SelfHost.ServiceRegistration
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add CORS policy (do this early in service configuration)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    // For development - allow your Vite/React dev server
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();

                    // For production - add your production frontend URL
                    // policy.WithOrigins("https://your-production-domain.com")
                });
            });

            services.AddControllers()
                .AddApplicationPart(Assembly.Load("SchoolPilot.Api"))
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.FeatureProviders.Add(new ControllerFeatureProvider());
                });

            services.AddEndpointsApiExplorer();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:ValidAudience"],
                    ValidateLifetime = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                        {
                            Console.WriteLine($"JWT Bearer Message Received. Token: {authHeader.Substring("Bearer ".Length)}");
                        }
                        else
                        {
                            Console.WriteLine("No Bearer token received in Authorization header.");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "SchoolPilot", Version = "v1" });
                c.SwaggerDoc("v2", new() { Title = "SchoolPilot", Version = "v2" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
                c.OperationFilter<AddRequiredHeadersFilter>();

                c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                c.OperationFilter<ApiVersionOperationFilter>();

                c.DocInclusionPredicate((version, apiDesc) =>
                {
                    if (!apiDesc.ActionDescriptor.EndpointMetadata.Any())
                        return false;

                    var versionAttribute = apiDesc.ActionDescriptor.EndpointMetadata
                        .OfType<ApiVersionAttribute>()
                        .FirstOrDefault();

                    return versionAttribute?.Versions.Any(v => $"v{v.ToString().Split('.')[0]}" == version) ?? false;
                });

                c.OperationFilter<RemoveVersionFromParameterFilter>();

            });

            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddMediatRServices();
            services.Load(configuration);
            services.AddAutoMapperService();
            services.AddDatabaseConnections(configuration);

            services.AddDbContext<ReadWriteSchoolPilotContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ReadWriteSchoolPilotDb")));

            services.AddScoped<SchoolPilotContext>(provider =>
                provider.GetRequiredService<ReadWriteSchoolPilotContext>());

            services.AddDbContext<ReadWriteSchoolPilotLookupContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ReadWriteSchoolPilotLookupDb")));

            services.AddScoped<SchoolPilotLookupContext>(provider =>
                provider.GetRequiredService<ReadWriteSchoolPilotLookupContext>());

            services.AddScoped<AccountController>();
            services.AddScoped<EnumController>();
            services.AddScoped<LookupController>();
            services.AddScoped<UserController>();
            services.AddScoped<AuthController>();
            services.AddScoped<SchoolSetupController>();
            services.AddScoped<SubjectController>();


            //Filter configurations (ensure these are registered as scoped services if they need DI)
            services.AddScoped<UserAuthorizeFilterAttribute>();
            services.AddScoped<AccountScopeFilterAttribute>();
            //services.AddScoped<CheckAuthTokenFilterAttribute>();
            services.AddScoped<TimeZoneFilterAttribute>();
            //services.AddScoped<UpdateUserFromTokenFilterAttribute>();
            //services.AddScoped<CheckUserRestrictionsFilterAttribute>();

            services.AddRefitClient<IAssetApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["AssetApi:BaseUrl"]);
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                });

            services.AddRefitClient<IAssetStorageApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["AssetStorageApi:BaseUrl"]);
                });

            services.AddTransient<IValidationErrorFactory, ValidationErrorFactory>();

            services.AddHangfire(config =>
            {
                config.UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UsePostgreSqlStorage(configuration.GetConnectionString("ReadWriteSchoolPilotDb"));
            });

            services.AddHangfireServer();


            services.AddTransient<IMethodExecutionHelper, MethodExecutionHelper>();

            var redisConfiguration = new RedisConfiguration()
            {
                AbortOnConnectFail = false,
                Hosts = new RedisHost[]
                {
                    new RedisHost
                    {
                        Host = configuration["Redis:Host"],
                        Port = int.Parse(configuration["Redis:Port"]) // Ensure this is parsed as int
                    }
                },
                Password = configuration["Redis:Password"],
                Database = int.Parse(configuration["Redis:Database"] ?? "0"),
                ConnectTimeout = int.Parse(configuration["Redis:ConnectTimeout"] ?? "5000")
            };

            services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(redisConfiguration);

            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                // This builds the multiplexer based on the configured options
                var options = ConfigurationOptions.Parse(
                    $"{redisConfiguration.Hosts[0].Host}:{redisConfiguration.Hosts[0].Port}," +
                    $"password={redisConfiguration.Password}," +
                    $"defaultDatabase={redisConfiguration.Database}," +
                    $"connectTimeout={redisConfiguration.ConnectTimeout}," +
                    $"abortConnect={redisConfiguration.AbortOnConnectFail}"
                );
                // Optionally add username if Redis 6+ ACL is used and username is provided
                //if (!string.IsNullOrEmpty(redisConfiguration.Username))
                //{
                //    options.User = redisConfiguration.Username;
                //}
                return ConnectionMultiplexer.Connect(options);
            });

            services.AddScoped<IRedisClient>(provider =>
            {
                return provider.GetRequiredService<IRedisClientFactory>().GetDefaultRedisClient();
            });

            var supabaseUrl = configuration["Supabase:Url"];
            var supabaseKey = configuration["Supabase:Key"];

            services.AddSingleton<Client>(_ =>
            new Client(
                supabaseUrl,
                supabaseKey,
                new SupabaseOptions { AutoConnectRealtime = true }));

            services.AddScoped<SupabaseStorageService>();
        }

        public static async Task SeedRolesAsync(ReadWriteSchoolPilotContext context)
        {
            var rolesToSeed = Enum.GetValues(typeof(UserRole))
                                  .Cast<UserRole>()
                                  .Select(role => new Data.Entities.Users.Role
                                  {
                                      Id = SequentialGuid.Create(),
                                      Name = role,
                                      NormalizedName = role.ToString().ToUpper()
                                  })
                .ToList();

            var existingRoles = await context.Set<Data.Entities.Users.Role>().ToListAsync();

            foreach (var role in rolesToSeed)
            {
                if (!existingRoles.Any(r => r.Name == role.Name))
                {
                    context.Add(role);
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedCountriesAsync(ReadWriteSchoolPilotLookupContext context)
        {
            var countriesToSeed = new List<Data.Entities.lookup.Country>
            {
                new Data.Entities.lookup.Country
                {
                    Id = SequentialGuid.Create(),
                    Code = CountryConstants.NigeriaCode,
                    Name = CountryConstants.Nigeria
                }
                // Add more countries here when needed
            };

            var existingCountries = await context.Set<Data.Entities.lookup.Country>().ToListAsync();

            foreach (var country in countriesToSeed)
            {
                if (!existingCountries.Any(c => c.Code == country.Code))
                {
                    context.Add(country);
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedStatesAsync(ReadWriteSchoolPilotLookupContext context)
        {
            var nigeria = await context.Set<Data.Entities.lookup.Country>()
                .FirstOrDefaultAsync(c => c.Code == CountryConstants.NigeriaCode);

            if (nigeria == null) return;

            var geoZones = new Dictionary<string, List<string>>
            {
                {
                    "North Central", new List<string>
                    {
                        StateConstants.Nigeria.Benue, StateConstants.Nigeria.Kogi,
                        StateConstants.Nigeria.Kwara, StateConstants.Nigeria.Nasarawa,
                        StateConstants.Nigeria.Niger, StateConstants.Nigeria.Plateau,
                        StateConstants.Nigeria.FCT
                    }
                },
                {
                    "North East", new List<string>
                    {
                        StateConstants.Nigeria.Adamawa, StateConstants.Nigeria.Bauchi,
                        StateConstants.Nigeria.Borno, StateConstants.Nigeria.Gombe,
                        StateConstants.Nigeria.Taraba, StateConstants.Nigeria.Yobe
                    }
                },
                {
                    "North West", new List<string>
                    {
                        StateConstants.Nigeria.Jigawa, StateConstants.Nigeria.Kaduna,
                        StateConstants.Nigeria.Kano, StateConstants.Nigeria.Katsina,
                        StateConstants.Nigeria.Kebbi, StateConstants.Nigeria.Sokoto,
                        StateConstants.Nigeria.Zamfara
                    }
                },
                {
                    "South East", new List<string>
                    {
                        StateConstants.Nigeria.Abia, StateConstants.Nigeria.Imo,
                        StateConstants.Nigeria.Ebonyi, StateConstants.Nigeria.Enugu,
                        StateConstants.Nigeria.Anambra
                    }
                },
                {
                    "South South", new List<string>
                    {
                        StateConstants.Nigeria.AkwaIbom, StateConstants.Nigeria.Bayelsa,
                        StateConstants.Nigeria.CrossRiver, StateConstants.Nigeria.Delta,
                        StateConstants.Nigeria.Edo, StateConstants.Nigeria.Rivers
                    }
                },
                {
                    "South West", new List<string>
                    {
                        StateConstants.Nigeria.Ekiti, StateConstants.Nigeria.Lagos,
                        StateConstants.Nigeria.Ogun, StateConstants.Nigeria.Ondo,
                        StateConstants.Nigeria.Osun, StateConstants.Nigeria.Oyo
                    }
                },

            };

            var statesToSeed = StateConstants.Nigeria.AllStates.Select(stateName =>
            {
                var region = geoZones.FirstOrDefault(zone => zone.Value.Contains(stateName)).Key;

                return new State
                {
                    Id = SequentialGuid.Create(),
                    Code = StateConstants.Nigeria.StateCodes.TryGetValue(stateName, out var code) ? code : string.Empty,
                    Name = stateName,
                    CountryId = nigeria.Id,
                    IsNigerianState = true,
                    Region = region
                };
            }).ToList();

            var existingStates = await context.Set<State>().ToListAsync();

            foreach (var state in statesToSeed)
            {
                if (!existingStates.Any(s => s.Name == state.Name && s.CountryId == state.CountryId))
                {
                    context.Add(state);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}