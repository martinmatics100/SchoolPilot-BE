using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhoneNumber = SchoolPilot.Data.Entities.PhoneNumber;

namespace SchoolPilot.Infrastructure.Queries.CustomEnums
{
    public class GetPhoneNumberType
    {
        public class Query : IRequest<Result>
        {

        }

        public class Result : IEnumerable<Model>
        {
            private readonly List<Model> _items;

            public Result(List<Model> items)
            {
                _items = items;
            }

            public IEnumerator<Model> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class Model
        {
            public string Name { get; set; }

            public int Value { get; set; }

            public bool HasExtension { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IMapper _mapper;

            public Handler(IMapper mapper)
            {
                _mapper = mapper;
            }

            public Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var enumValues = EnumExtensions.GetValues<PhoneType>();
                var result = _mapper.Map<List<Model>>(enumValues);

                foreach (var item in result.Where(x => PhoneExtensionType.Contains(x.Value)))
                {
                    item.HasExtension = true;
                }

                return Task.FromResult(new Result(result));
            }

            private static HashSet<int> PhoneExtensionType => new HashSet<int>()
            {
                ((int)PhoneType.Office),
                ((int)PhoneType.Facility),
                ((int)PhoneType.Work)
            };
        }
    }
}