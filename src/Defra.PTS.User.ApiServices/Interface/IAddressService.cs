using Defra.PTS.User.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model = Defra.PTS.User.Models;

namespace Defra.PTS.User.ApiServices.Interface
{
    public interface IAddressService
    {
        Task<Guid> CreateAddress(model.Address addressModel);

        Task<model.Address> GetAddressModel(Stream addressStream);
    }
}
