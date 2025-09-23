using EgrWebEntity.ModelTable;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Npgsql.EntityFrameworkCore.PostgreSQL;
namespace AuthTest.Server
{
    public class DbContextTable : DbContext
    {
        #region Таблицы
        public DbSet<EGRIPSVFL> ЕГРИП_СвФЛ { get; set; }
        public DbSet<EGRIPOKVED> ЕГРИП_ОКВЭД { get; set; }
        public DbSet<EGRIPSvAdrMJ> ЕГРИП_СвАдрМЖ { get; set; }
        public DbSet<EGRIPSvGrajd> ЕГРИП_СвГражд { get; set; }
        public DbSet<EGRIPSvIP> ЕГРИП_СвИП { get; set; }
        public DbSet<EGRIPSvLicense> ЕГРИП_СвЛицензия { get; set; }
        public DbSet<EGRIPSvPrekras_> ЕГРИП_СвПрекращ { get; set; }
        public DbSet<EGRIPSvRegIP> ЕГРИП_СвРегИП { get; set; }
        public DbSet<EGRIPSvGegOrg> ЕГРИП_СвРегОрг { get; set; }
        public DbSet<EGRIPSvRegPF> ЕГРИП_СвРегПФ { get; set; }
        public DbSet<EGRIPSvRegFSS> ЕГРИП_СвРегФСС { get; set; }
        public DbSet<EGRIPSvAccountingNO> ЕГРИП_СвУчетНО { get; set; }
        public DbSet<EGRULOKVED> ЕГРЮЛ_ОКВЭД { get; set; }
        public DbSet<EGRULSvAddressUL> ЕГРЮЛ_СвАдресЮЛ { get; set; }
        public DbSet<EGRULSvDerjRegistryAO> ЕГРЮЛ_СвДержРеестрАО { get; set; }
        public DbSet<EGRULSvShareOOO> ЕГРЮЛ_СвДоляООО { get; set; }
        public DbSet<EGRULSvZapEGRUL> ЕГРЮЛ_СвЗапЕГРЮЛ { get; set; }
        public DbSet<EGRULSvLicense> ЕГРЮЛ_СвЛицензия { get; set; }
        public DbSet<EGRULSvNaimUL> ЕГРЮЛ_СвНаимЮЛ { get; set; }
        public DbSet<EGRULSvObrUL> ЕГРЮЛ_СвОбрЮЛ { get; set; }
        public DbSet<EGRULSvPodrazd> ЕГРЮЛ_СвПодразд { get; set; }
        public DbSet<EGRULSvPredsh> ЕГРЮЛ_СвПредш { get; set; }
        public DbSet<EGRULSvPreem> ЕГРЮЛ_СвПреем { get; set; }
        public DbSet<EGRULSvPrekrUL> ЕГРЮЛ_СвПрекрЮЛ { get; set; }
        public DbSet<EGRULSvRegOrg> ЕГРЮЛ_СвРегОрг { get; set; }
        public DbSet<EGRULSvRegPF> ЕГРЮЛ_СвРегПФ { get; set; }
        public DbSet<EGRULSvRegFSS> ЕГРЮЛ_СвРегФСС { get; set; }
        public DbSet<EGRULSvReorg> ЕГРЮЛ_СвРеорг { get; set; }
        public DbSet<EGRULSvStatus> ЕГРЮЛ_СвСтатус { get; set; }
        public DbSet<EGRULSvUstKap> ЕГРЮЛ_СвУстКап { get; set; }
        public DbSet<EGRULSvAccountingNO> ЕГРЮЛ_СвУчетНО { get; set; }
        public DbSet<EGRULSvFounder> ЕГРЮЛ_СвУчредит { get; set; }
        public DbSet<EGRULSvUL> ЕГРЮЛ_СвЮЛ { get; set; }
        public DbSet<EGRULSvedDoljnFL> ЕГРЮЛ_СведДолжнФЛ { get; set; }
        public DbSet<Document> Документ { get; set; }
        public DbSet<IP> ИП { get; set; }
        public DbSet<UL> ЮрЛицо { get; set; }
        public DbSet<ChangeLog> ИсторияИзменений { get; set; }
        public DbSet<Users> Пользователи { get; set; }
        #endregion

        public DbContextTable(DbContextOptions<DbContextTable> options) : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = WebApplication.CreateBuilder();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }

    }

    public static class MyExtensions
    {
        public static IQueryable Set(this DbContext _context, Type t)
        {
            return (IQueryable)_context.GetType().GetMethods()
        .Where(x => x.Name == "Set")
        .FirstOrDefault(x => x.IsGenericMethod)
        .MakeGenericMethod(t)
        .Invoke(_context, null);
        }
    }
}
