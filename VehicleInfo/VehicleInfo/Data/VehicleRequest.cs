using System.ComponentModel.DataAnnotations;

namespace VechicleInfo.Data
{
    public class VehicleRequest
    {
        //Registration Number format Check: AA11AAA (2 alphabets followed by 2 numbers followed by 3 aplhabets) 
        [Required]
        [RegularExpression(@"^[A-Za-z]{2}\d{2}[A-Za-z]{3}$", ErrorMessage = "Invalid registration number format.")]
        public string RegistrationNumber { get; set; }
    }
}
