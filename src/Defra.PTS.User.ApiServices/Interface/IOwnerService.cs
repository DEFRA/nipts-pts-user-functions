using model = Defra.PTS.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.ApiServices.Interface
{
    public interface IOwnerService
    {
        Task<model.Owner> GetOwnerModel(Stream userStream);
        Task<bool> DoesOwnerExists(string ownerEmail);
        Task<Owner> GetOwnerByEmail(string ownerEmail);
        Task<Guid> CreateOwner(model.Owner ownerModel);
    }
}
