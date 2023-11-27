using VechicleInfo.Data;

namespace VechicleInfo.Services
{
    
        public interface IVehicleService
        {
            Task<VehicleInformation> GetVehicleInfo(string registrationNumber);
        }
    
}
