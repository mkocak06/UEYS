using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.ResponseModels;

namespace Infrastructure.Data
{
    public class TitleRepository : EfRepository<Title>, ITitleRepository
    {
        private readonly ApplicationDbContext dbContext;

        public TitleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<TitleResponseDTO> QueryableTitles()
        {
            var titles = dbContext.Titles.Select(x => new TitleResponseDTO()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            TitleType = x.TitleType,
                            TitleTypeName = x.TitleType.GetDescription()
                        }).ToList();

            return titles.AsQueryable();
        }
    }
}