using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class MaterializedViewService : IMaterializedViewService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmailSender _emailSender;

    public MaterializedViewService(ApplicationDbContext dbContext, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _emailSender = emailSender;
    }

    public async Task UpdateMvTcknViewAsync()
    {
        using (var transaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW kds_integration.mv_tckn_view;");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // If any error occurs, roll back the transaction
                await transaction.RollbackAsync();

                //EmailMessage model = new()
                //{
                //    To = { "ismail.topcam@saglik.gov.tr", "mehmet.kocak13@saglik.gov.tr" },
                //    Subject = "mv_tckn_view Güncelleme Hatası",
                //    Body = $"Error updating materialized view: {ex.Message}",
                //};

                //_emailSender.SendEmail(model);

                //throw new Exception("Error updating materialized view: " + ex.Message, ex);
            }
        }
    }

    public async Task UpdateMvKurumViewAsync()
    {
        using (var transaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW kds_integration.mv_kurum_view;");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // If any error occurs, roll back the transaction
                await transaction.RollbackAsync();

                //EmailMessage model = new()
                //{
                //    To = { "ismail.topcam@saglik.gov.tr", "mehmet.kocak13@saglik.gov.tr" },
                //    Subject = "mv_kurum_view Güncelleme Hatası",
                //    Body = $"Error updating materialized view: {ex.Message}",
                //};

                //_emailSender.SendEmail(model);

                //throw new Exception("Error updating materialized view: " + ex.Message, ex);
            }
        }
    }
}
