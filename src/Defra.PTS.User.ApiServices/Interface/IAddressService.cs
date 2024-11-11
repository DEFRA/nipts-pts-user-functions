using Defra.PTS.User.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Defra.PTS.User.Models;

namespace Defra.PTS.User.ApiServices.Interface
{
    public interface IAddressService
    {
        Task<Guid> CreateAddress(Model.Address addressModel);

        Task<Model.Address> GetAddressModel(Stream addressStream);
    }
}
