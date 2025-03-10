using Core.Entities;
using System.Linq;
using Shared.ResponseModels;

namespace Core.Interfaces
{
    public interface ITitleRepository : IRepository<Title>
    {
        IQueryable<TitleResponseDTO> QueryableTitles();
    }
}
