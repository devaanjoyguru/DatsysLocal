﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data.Entity.Core.Objects;

namespace DATSYS.TradingModel.DataEntitySchema
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class DatsystemsEntities : DbContext
    {
        public DatsystemsEntities()
            : base("name=DatsystemsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Bar> Bars { get; set; }
        public DbSet<DailyPriceBar> DailyPriceBars { get; set; }
        public DbSet<StagingTickData> StagingTickDatas { get; set; }
        public DbSet<RegressionJob> RegressionJobs { get; set; }
    
        public virtual int RegressionJob_Insert(string instrumentCode, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> barInterval, string strategyName, Nullable<bool> isDaily)
        {
            var instrumentCodeParameter = instrumentCode != null ?
                new ObjectParameter("instrumentCode", instrumentCode) :
                new ObjectParameter("instrumentCode", typeof(string));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var barIntervalParameter = barInterval.HasValue ?
                new ObjectParameter("barInterval", barInterval) :
                new ObjectParameter("barInterval", typeof(int));
    
            var strategyNameParameter = strategyName != null ?
                new ObjectParameter("strategyName", strategyName) :
                new ObjectParameter("strategyName", typeof(string));
    
            var isDailyParameter = isDaily.HasValue ?
                new ObjectParameter("isDaily", isDaily) :
                new ObjectParameter("isDaily", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("RegressionJob_Insert", instrumentCodeParameter, startDateParameter, endDateParameter, barIntervalParameter, strategyNameParameter, isDailyParameter);
        }
    
        public virtual int RegressionJob_SetFinished(Nullable<int> jobid)
        {
            var jobidParameter = jobid.HasValue ?
                new ObjectParameter("jobid", jobid) :
                new ObjectParameter("jobid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("RegressionJob_SetFinished", jobidParameter);
        }
    }
}