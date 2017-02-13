using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWinService
{
    public partial class DemoWinService : ServiceBase
    {
        shortenURLEntities dc=new shortenURLEntities();
        System.Threading.Timer _timer;
        public DemoWinService()
        {
            //_timer = new System.Threading.Timer(
            //new TimerCallback(OnTimerCallBack), null, Timeout.Infinite, Timeout.Infinite);
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
           // MSYNC();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 10000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            // _timer.Change(0, 2000);
        }
        protected void timer_Elapsed(object source, System.Timers.ElapsedEventArgs aa)
        {
            MSYNC();
        }
        protected override void OnStop()
        {
           // LogMessage("Service Stopped - ");
        }
        //public void OnTimerCallBack(object state)
        //{
        //    //LogMessage("Timer Tick - ");
        //    MSYNC();
        //}
        public void MSYNC()
        {
            try
            {
                long? rowUpated = 0; List<List_IP> NotFoundIps = new List<List_IP>(); List<CountryCityModel> list_CountryCity = new List<CountryCityModel>();
                long? lastrow_updated_id = dc.tmp_rownum_update.Select(x => x.row_update).SingleOrDefault();
                List<List_IP> ipobj = (from s in dc.SHORTURLDATAs
                                       where s.PK_Shorturl > lastrow_updated_id
                                       select new List_IP()
                                       {
                                           ipnum = s.ip_num,
                                           ipaddress = s.Ipv4,
                                           pk_shorturl_id = s.PK_Shorturl,
                                           userAgent=s.UserAgent
                                       }).ToList();
                //check if any records are there for updation
                if (ipobj.Count != 0)
                {
                    //get matched records from master_location table
                    List<locids> locids = ipobj.Where(i => dc.Master_Location.AsNoTracking().Any(foo => i.ipnum >= foo.startIpNum && i.ipnum <= foo.endIpNum)).Select(x => new locids() {userAgent=x.userAgent, ipnum = x.ipnum, pk_shorturl_id = x.pk_shorturl_id, localid = dc.Master_Location.Where(foo => x.ipnum >= foo.startIpNum && x.ipnum <= foo.endIpNum).Select(y => y.locId).FirstOrDefault(), fk_city_master_id = dc.Master_Location.Where(foo => x.ipnum >= foo.startIpNum && x.ipnum <= foo.endIpNum).Select(y => y.PK_MASTERID).FirstOrDefault() }).ToList();

                    if (locids.Count != 0)
                    {
                        //get records from location_data table
                        list_CountryCity = (from l in dc.Locations_Data

                                                                 .AsNoTracking()
                                                                 .AsEnumerable() // Continue in memory
                                            join i in locids on l.locId equals i.localid
                                            where l.locId == i.localid

                                            select new CountryCityModel()
                                            {
                                                ipnum = i.ipnum,
                                                Country = GetCountryName(l.country),
                                                CountryCode = l.country,
                                                City = l.city,
                                                Region = l.region,
                                                PostalCode = l.postalCode,
                                                latitude = l.latitude,
                                                longitude = l.longitude,
                                                metro_code = l.metroCode,
                                                fk_city_master_id = i.fk_city_master_id,
                                                pk_shorturl_id = i.pk_shorturl_id,
                                                userAgent=i.userAgent


                                            }).ToList();
                        if (list_CountryCity.Count != 0)
                        {
                            //check if any ips not found in databaase tables
                            if (list_CountryCity.Count != ipobj.Count)
                            {
                                NotFoundIps = ipobj.Where(p => !list_CountryCity.Any(p2 => p2.ipnum == p.ipnum)).ToList();
                            }
                            //foreach (CountryCityModel i in list_CountryCity)
                            //{
                            //    new DataInsertionBO().UpdateCityCountry(i);
                            //}

                        }
                        else
                        {
                            NotFoundIps = ipobj;

                        }
                        if (NotFoundIps.Count != 0)
                        {
                            //get data from freegeoip service and save data for future use
                            List<CountryCityModel> list = new DataInsertionBO().GetDataForNotFoundIPS(NotFoundIps);
                            list_CountryCity = list_CountryCity.Concat(list).ToList();
                        }

                    }
                    else
                    {
                        //if no record found in master_location table
                        NotFoundIps = ipobj;

                        List<CountryCityModel> list = new DataInsertionBO().GetDataForNotFoundIPS(NotFoundIps);
                        list_CountryCity = list_CountryCity.Concat(list).ToList();

                    }
                    foreach (CountryCityModel i in list_CountryCity)
                    {
                        new DataInsertionBO().UpdateCityCountry(i);
                    }
                    List_IP lastrecord = ipobj[ipobj.Count - 1];
                    rowUpated = lastrecord.pk_shorturl_id;
                    tmp_rownum_update obj = new tmp_rownum_update();
                    obj.row_update = rowUpated;
                    new DataInsertionBO().UpdateRowid(rowUpated);
                }
            }
            catch (Exception ex)
            {

                ErrorLogs.LogErrorData(ex.StackTrace, ex.InnerException.ToString());

            }
            //List<CountryCityModel> ipdata = (from m in dc.Master_Location
            //              join l in dc.Locations_Data on m.locId equals l.locId
            //              select new CountryCityModel()
            //              {
            //                  startIpNum=m.startIpNum,
            //                  endIpNum=m.endIpNum,
            //                  //Country=GetCountryName(l.country),
            //                  CountryCode=l.country,
            //                  City=l.city,
            //                  Region=l.region,
            //                  PostalCode=l.postalCode,
            //                  latitude=l.latitude,
            //                  longitude=l.longitude,
            //                  metro_code=l.metroCode,
            //                  fk_city_master_id=m.PK_MASTERID
            //                 // pk_shorturl_id=m.PK_MASTERID
            //              }).ToList();

            //var lobj = ipdata.Where(i => ipobj.Any(foo => foo.ipnum >= i.startIpNum && foo.ipnum <= i.endIpNum)).
            //                          Select(l => new CountryCityModel()
            //                          {
            //                              Country = GetCountryName(l.CountryCode),
            //                              CountryCode = l.CountryCode,
            //                              City = l.City,
            //                              Region = l.Region,
            //                              PostalCode = l.PostalCode,
            //                              latitude = l.latitude,
            //                              longitude = l.longitude,
            //                              metro_code = l.metro_code,
            //                              fk_city_master_id = l.fk_city_master_id,
            //                              pk_shorturl_id = l.pk_shorturl_id
            //                              // pk_shorturl_id=(from i in ipobj select i.pk_shorturl_id).Single()}).ToList();
            //                          }).ToList();



        }
        public string GetCountryName(string CountryCode)
        {
            RegionInfo cultureInfo = new RegionInfo(CountryCode);
            string CountryName = cultureInfo.EnglishName;
            return CountryName;
        }

    }
}
