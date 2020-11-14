using System;

namespace API.Extensions
{
    public static class DateTimeExtension
    {
        public static int CalculateAge(this DateTime DOB){
            int Age = DateTime.Today.Year - DOB.Year;
            //-1 when birthdate not reach
            if( DOB.AddYears(Age) > DateTime.Today)
                Age = Age - 1;
            return Age;
        }
    }
}