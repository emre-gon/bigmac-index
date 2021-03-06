using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BigMacReporter.TaxSystem
{
    public abstract class ITaxSystem
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Three letter code</returns>
        public abstract string CountryCode();


        public abstract decimal CalculateAnnualNetWage(decimal AnnualGrossWage, DateTime date);

        public static decimal GetRateFromDict(decimal TaxableIncome, SortedDictionary<long,double> rates)
        {
            foreach (var rate in rates)
            {
                decimal maxLimit = rate.Key;

                if (TaxableIncome < maxLimit)
                {
                    return (decimal)rate.Value;
                }
            }

            return (decimal)rates.Last().Value;
        }

        public WageModel GetAnnualNetWage(WageModel input, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now.Date;

            var annualWage = GetAnnualWage(input);

            annualWage.NetLocalPrice = CalculateAnnualNetWage(annualWage.GrossLocalPrice, date.Value);


            return annualWage;
        }

        public WageModel GetMonthlyNetWage(WageModel input, DateTime? date = null)
        {
            return GetMonthlyWage(GetAnnualNetWage(input));
        }



        public WageModel GetAnnualWage(WageModel input)
        {
            if (input.Type == Domain.MinWageType.Annually)
                return input;


            WageModel tbr = new WageModel()
            {
                Type = Domain.MinWageType.Annually,
                Profession = input.Profession,
                Date = input.Date,
                TaxWedge = input.TaxWedge
            };

            if (input.Type == Domain.MinWageType.Monthly)
            {
                tbr.GrossLocalPrice = input.GrossLocalPrice * 12;
                tbr.NetLocalPrice = input.NetLocalPrice * 12;
            }
            else if(input.Type == Domain.MinWageType.Daily)
            {
                tbr.GrossLocalPrice = input.GrossLocalPrice * GetTotalAnnualWorkDays();
                tbr.NetLocalPrice = input.NetLocalPrice * GetTotalAnnualWorkDays();
            }
            else if (input.Type == Domain.MinWageType.Hourly)
            {
                tbr.GrossLocalPrice = input.GrossLocalPrice * GetTotalAnnualWorkHours();
                tbr.NetLocalPrice = input.NetLocalPrice * GetTotalAnnualWorkHours();
            }

            return tbr;
        }


        public virtual int GetTotalAnnualWorkHours()
        {
            return 1950;
        }

        public virtual int GetTotalAnnualWorkDays()
        {
            return 253;
        }

        public WageModel GetMonthlyWage(WageModel input)
        {
            var wage = GetAnnualWage(input);

            WageModel wr = new WageModel()
            {
                Date = wage.Date,
                GrossLocalPrice = wage.GrossLocalPrice / 12,
                NetLocalPrice = wage.NetLocalPrice / 12,
                TaxWedge = wage.TaxWedge,
                Type = Domain.MinWageType.Monthly,
                Profession = wage.Profession,


            };

            return wr;

        }
    }
}
