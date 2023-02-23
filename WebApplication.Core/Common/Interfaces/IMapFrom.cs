using AutoMapper;

namespace WebApplication.Core.Common.Interfaces
{
    public interface IMapFrom<T>
    {
        /// <summary>
        /// Creates a default mapping profile from the source type T to the destination type (class that implements this interface)
        /// </summary>
        /// <param name="profile">An AutoMapper <see cref="Profile"/></param>
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
