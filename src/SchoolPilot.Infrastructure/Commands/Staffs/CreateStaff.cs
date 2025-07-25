

//using MediatR;
//using SchoolPilot.Common.Enums;

//namespace SchoolPilot.Infrastructure.Commands.Staffs
//{
//    public static class CreateStaff
//    {
//        public class Command : IRequest<Result>
//        {
//            public string FirstName { get; set; }
//            public string LastName { get; set; }
//            public string Email { get; set; }
//            public EmploymentType EmploymentType { get; set; }
//            public Gender Gender { get; set; }
//            public MaritalStatus MaritalStatus { get; set; }
//            public Nationality Nationality { get; set; }
//            public string Notes { get; set; }

//            public PhoneNumberModel PhoneNumber {  get; set; }

//            public AddressModel Address { get; set; }
//        }

//        public class PhoneNumberModel
//        {
//            public string Number { get; set; }
//            public string Extension { get; set; }
//            public PhoneType PhoneType { get; set; }
//            public Country CountryCode { get; set; }
//        }

//        public class AddressModel
//        {
//            public string AddressLine { get; set; }

//            public string City {  get; set; }
//            public string Country { get; set; }

//            public string PostalCode { get; set; }
//            public string State { get; set; }
//        }
//    }
//}
