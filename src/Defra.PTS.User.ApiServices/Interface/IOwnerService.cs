using Model = Defra.PTS.User.Models;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.ApiServices.Interface
{
    public interface IOwnerService
    {
        Task<Model.Owner> GetOwnerModel(Stream userStream);
        Task<bool> DoesOwnerExists(string ownerEmail);
        Task<Owner?> GetOwnerByEmail(string ownerEmail);
        Task<Guid> CreateOwner(Model.Owner ownerModel);
        Task UpdateOwnerEmailsByOldEmail(string oldEmail, string newEmail);
    }
}
