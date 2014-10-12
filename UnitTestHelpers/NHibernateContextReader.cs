using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NHibernate;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace UnitTestHelpersNS
{
	public static class NHibernateContextReader
	{
		
	}
}
/*
using DealerOn.Shared.NHDataLayer.Inventory.Tables;
using DealerOn.Shared.NHDataLayer.Refdata.Tables;
using NHibernate;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DealerOnInv.UnitTests
{
  public class ReadTestData
  {
    public static string LowerCaseTags(string xml)
    {
      return Regex.Replace(
        xml,
        @"<[^<>]+>",
        m => { return m.Value.ToLower(); },
        RegexOptions.Multiline | RegexOptions.Singleline);
    }

    public static void Read(ISession db, string xmlFileName)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = "DealerOnInv.UnitTests.TestData." + xmlFileName;
      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream == null)
        {
          throw new Exception("Cannot find XML data file, make sure it is set to Embedded Resource!");
        }

        using (StreamReader reader = new StreamReader(stream))
        {
          string xmlData = LowerCaseTags(reader.ReadToEnd());
          XDocument document = XDocument.Parse(xmlData);

          #region tDms table

          foreach (XElement element in document.Descendants("tdms"))
          {
            var tdms = new Dms
              {
                DmsId = ReadIntXmlData(element, "dmsid") ?? 0,
                Name = ReadStringXmlData(element, "name"),
                Enabled = ReadBoolXmlData(element, "enabled"),
                ManualControl = ReadStringXmlData(element, "manualcontrol"),
                PhotoProvider = ReadIntXmlData(element, "photoprovider"),
                LastUpdated = ReadDateXmlData(element, "lastupdated"),
                DmsImportStatusId = ReadIntXmlData(element, "dmsimportstatusid") ?? 0,
                DmsImportStatusDate = ReadDateXmlData(element, "dmsimportstatusdate"),
                CurrentProcessor = ReadStringXmlData(element, "currentprocessor"),
                TotalProcessTime = ReadIntXmlData(element, "totalprocesstime"),
                Extended = ReadStringXmlData(element, "extended"),
                Error = ReadStringXmlData(element, "error"),
                VideoCol = ReadIntXmlData(element, "videocol"),
                VideoFeedId = ReadIntXmlData(element, "videofeedid"),
                TagCol = ReadIntXmlData(element, "tagcol")
              };
            db.Save(tdms);
            db.Flush();
          }

          #endregion

          #region tDmsImportStatus table

          foreach (XElement element in document.Descendants("tdmsimportstatus"))
          {
            var dmsimportstatus = new DmsImportStatus
              {
                DmsImportStatusId = ReadIntXmlData(element, "dmsimportstatusid") ?? 0,
                StatusName = ReadStringXmlData(element, "statusname")
              };
            db.Save(dmsimportstatus);
            db.Flush();
          }

          #endregion

          #region tdmsjobs table

          foreach (XElement element in document.Descendants("tdmsjobs"))
          {
            var dmsjobs = new DmsJobs
              {
                DmsId = ReadIntXmlData(element, "dmsid") ?? 0,
                ImportJobId = ReadIntXmlData(element, "importjobid") ?? 0,
                ImportJobTypeId = ReadIntXmlData(element, "importjobtypeid") ?? 0,
                FeedDealerId = ReadStringXmlData(element, "feeddealerid"),
                Enabled = ReadBoolXmlData(element, "enabled") ?? false,
                Sequence = ReadIntXmlData(element, "sequence"),
                ImportDate = ReadDateXmlData(element, "importdate"),
                Error = ReadStringXmlData(element, "error"),
                CurrentProcessor = ReadStringXmlData(element, "currentprocessor"),
                DataImported = ReadBoolXmlData(element, "dataimported")
              };
            db.Save(dmsjobs);
            db.Flush();
          }

          #endregion

          #region timportjobtypes table

          foreach (XElement element in document.Descendants("timportjobtypes"))
          {
            var importJobTypes = new ImportJobTypes
              {
                ImportJobTypeId = ReadIntXmlData(element, "importjobtypeid") ?? 0,
                Name = ReadStringXmlData(element, "name"),
                UpdateOnly = ReadBoolXmlData(element, "updateonly")
              };
            db.Save(importJobTypes);
            db.Flush();
          }

          #endregion


          #region tdmsrushjob table

          foreach (XElement element in document.Descendants("tdmsrushjob"))
          {
            var dmsrushjob = new DmsRushJob
              {
                dmsrushjob = ReadIntXmlData(element, "dmsrushjob") ?? 0,
                dmsid = ReadIntXmlData(element, "dmsid"),
                ImportCompleted = ReadBoolXmlData(element, "importcompleted")
              };
            db.Save(dmsrushjob);
            db.Flush();
          }

          #endregion

          #region tfeaturechoicerules

          foreach (XElement element in document.Descendants("tfeaturechoicerules"))
          {
            var featurechoicerules = new FeatureChoiceRules
              {
                Id = ReadIntXmlData(element, "id") ?? 0,
                FeatureChoice = ReadIntXmlData(element, "featurechoice") ?? 0,
                Field = ReadStringXmlData(element, "field"),
                Value = ReadStringXmlData(element, "value")
              };
            db.Save(featurechoicerules);
            db.Flush();
          }

          #endregion

          #region timportjobs table

          foreach (XElement element in document.Descendants("timportjobs"))
          {
            var importjobs = new ImportJobs
              {
                ImportJobId = ReadIntXmlData(element, "importjobid") ?? 0,
                Name = ReadStringXmlData(element, "name"),
                Description = ReadStringXmlData(element, "description"),
                Enabled = ReadBoolXmlData(element, "enabled") ?? false,
                LastRunFullGet = ReadBoolXmlData(element, "lastrunfullget"),
                LastRunOutcome = ReadStringXmlData(element, "lastrunoutcome"),
                LastRunTime = ReadDateXmlData(element, "lastruntime"),
                NextRunTime = ReadDateXmlData(element, "nextruntime"),
                NextRunFullGet = ReadBoolXmlData(element, "nextrunfullget"),
                CurrentStatusId = ReadIntXmlData(element, "currentstatusid"),
                CurrentProcessor = ReadStringXmlData(element, "currentprocessor"),
                UpdatedDate = ReadDateXmlData(element, "updateddate"),
                ProviderMapProc = ReadStringXmlData(element, "providermapproc"),
                DefaultJobTypeId = ReadIntXmlData(element, "defaultjobtypeid") ?? 0,
                TotalProcessTime = ReadIntXmlData(element, "totalprocesstime"),
                LastCsvImportDate = ReadDateXmlData(element, "lastcsvimportdate"),
                DestinationFilename = ReadStringXmlData(element, "destinationfilename")
              };
            db.Save(importjobs);
            db.Flush();
          }

          #endregion

          #region timportlogs table

          foreach (XElement element in document.Descendants("timportlogs"))
          {
            var importlogs = new ImportLogs
              {
                ImportLogId = ReadIntXmlData(element, "importlogid") ?? 0,
                ImportJobId = ReadIntXmlData(element, "importjobid") ?? 0,
                ImportDate = ReadDateXmlData(element, "importdate") ?? DateTime.Now,
                Success = ReadBoolXmlData(element, "success"),
                Retry = ReadIntXmlData(element, "retry")
              };
            db.Save(importlogs);
            db.Flush();
          }

          #endregion

          #region timportschedules table

          foreach (XElement element in document.Descendants("timportschedules"))
          {
            var importschedules = new ImportSchedules
              {
                ImportScheduleId = ReadIntXmlData(element, "importscheduleid") ?? 0,
                ImportJobId = ReadIntXmlData(element, "importjobid") ?? 0,
                ScheduleName = ReadStringXmlData(element, "schedulename"),
                FullGet = ReadBoolXmlData(element, "fullget"),
                TimeFrequency = ReadIntXmlData(element, "timefrequency") ?? 0,
                FreqStartTime = ReadDateXmlData(element, "freqstarttime"),
                FreqEndDate = ReadDateXmlData(element, "freqenddate")
              };
            db.Save(importschedules);
            db.Flush();
          }

          #endregion

          #region timportsources table

          foreach (XElement element in document.Descendants("timportsources"))
          {
            var importsources = new ImportSources
              {
                ImportSourceId = ReadIntXmlData(element, "importsourceid") ?? 0,
                ImportJobId = ReadIntXmlData(element, "importjobid"),
                FileName = ReadStringXmlData(element, "filename"),
                FileFormat = ReadStringXmlData(element, "fileformat"),
                FileType = ReadStringXmlData(element, "filetype"),
                FieldSeparator = ReadStringXmlData(element, "fieldseparator"),
                TechSpecsSeparator = ReadStringXmlData(element, "techspecsseparator"),
                WarrantySeparator = ReadStringXmlData(element, "warrantyseparator"),
                FeatureSeparator = ReadStringXmlData(element, "featureseparator"),
                OptionSeparator = ReadStringXmlData(element, "optionseparator"),
                StartEndSeparator = ReadStringXmlData(element, "startendseparator"),
                OtherSeparator1 = ReadStringXmlData(element, "otherseparator1"),
                OtherSeparator2 = ReadStringXmlData(element, "otherseparator2"),
                SkipFirstLine = ReadBoolXmlData(element, "skipfirstline"),
                IsBackup = ReadBoolXmlData(element, "isbackup"),
                Enabled = ReadBoolXmlData(element, "enabled"),
                ProtocolType = ReadStringXmlData(element, "protocoltype"),
                ProtocolSource = ReadStringXmlData(element, "protocolsource"),
                ProtocolUsername = ReadStringXmlData(element, "protocolusername"),
                ProtocolPassword = ReadStringXmlData(element, "protocolpassword"),
                Fields = ReadStringXmlData(element, "fields"),
                Token = ReadStringXmlData(element, "token"),
                NumberRetry = ReadIntXmlData(element, "numberretry"),
                TimeRetry = ReadIntXmlData(element, "timeretry"),
                ImportJobTypeId = ReadIntXmlData(element, "importjobtypeid")
              };
            db.Save(importsources);
            db.Flush();
          }

          #endregion

          #region timportstatus table

          foreach (XElement element in document.Descendants("timportstatus"))
          {
            var importstatus = new ImportStatus
              {
                ImportStatusId = ReadIntXmlData(element, "importstatusid") ?? 0,
                SourceStatus = ReadStringXmlData(element, "sourcestatus")
              };
            db.Save(importstatus);
            db.Flush();
          }

          #endregion

          #region tinventoryimport table

          foreach (XElement element in document.Descendants("tinventoryimport"))
          {
            var inventoryimport = new InventoryImport
              {
                ExtColor = ReadStringXmlData(element, "extcolor"),
                StockNum = ReadStringXmlData(element, "stocknum"),
                Mileage = ReadFloatXmlData(element, "mileage"),
                VehicleType = ReadStringXmlData(element, "vehicletype"),
                VehicleYear = ReadStringXmlData(element, "vehicleyear"),
                VehicleMake = ReadStringXmlData(element, "vehiclemake"),
                VehicleModel = ReadStringXmlData(element, "vehiclemodel"),
                VehicleTrim = ReadStringXmlData(element, "vehicletrim"),
                Days = ReadStringXmlData(element, "days"),
                Msrp = ReadFloatXmlData(element, "msrp"),
                Invoice = ReadFloatXmlData(element, "invoice"),
                Cost = ReadStringXmlData(element, "cost"),
                VehicleId = ReadIntXmlData(element, "vehicleid"),
                Comments = ReadStringXmlData(element, "comments"),
                BodyStyle = ReadStringXmlData(element, "bodystyle"),
                InternetPrice = ReadFloatXmlData(element, "internetprice"),
                Display = ReadBoolXmlData(element, "display"),
                Edited = ReadBoolXmlData(element, "edited"),
                Options = ReadStringXmlData(element, "options"),
                Cpo = ReadBoolXmlData(element, "cpo"),
                Engine = ReadStringXmlData(element, "engine"),
                Transmission = ReadStringXmlData(element, "transmission"),
                IntColor = ReadStringXmlData(element, "intcolor"),
                Photo = ReadStringXmlData(element, "photo"),
                PhotoIsStock = ReadBoolXmlData(element, "photoisstock"),
                PhotoColorMatching = ReadBoolXmlData(element, "photocolormatching"),
                ExtColorGeneric = ReadStringXmlData(element, "extcolorgeneric"),
                MpgCity = ReadIntXmlData(element, "mpgcity"),
                MpgHwy = ReadIntXmlData(element, "mpghwy"),
                EngineCylinders = ReadStringXmlData(element, "enginecylinders"),
                EngineDisp = ReadStringXmlData(element, "enginedisp"),
                Special = ReadIntXmlData(element, "special"),
                Carfax1Owner = ReadBoolXmlData(element, "carfax1owner"),
                ExtSpecial = ReadBoolXmlData(element, "extspecial"),
                Status = ReadIntXmlData(element, "status"),
                InventoryDt = ReadDateXmlData(element, "inventorydt"),
                PriceChangeDt = ReadDateXmlData(element, "pricechangedt"),
                SoldDt = ReadDateXmlData(element, "solddt"),
                UpdatedDt = ReadDateXmlData(element, "updateddt"),
                CreatedDt = ReadDateXmlData(element, "createddt"),
                PrevPrice = ReadFloatXmlData(element, "prevprice"),
                Tags = ReadStringXmlData(element, "tags"),
                Sort = ReadIntXmlData(element, "sort"),
                ModelCode = ReadStringXmlData(element, "modelcode"),
                Body1 = ReadStringXmlData(element, "body1"),
                Body2 = ReadStringXmlData(element, "body2"),
                Transmission1 = ReadStringXmlData(element, "transmission1"),
                Transmission2 = ReadStringXmlData(element, "transmission2"),
                Engine1 = ReadStringXmlData(element, "engine1"),
                Engine2 = ReadStringXmlData(element, "engine2"),
                LocationId = ReadIntXmlData(element, "locationid"),
                SpecialsId = ReadIntXmlData(element, "specialsid"),
                PhotoUpdateDate = ReadDateXmlData(element, "photoupdatedate"),
                TechSpecs = ReadStringXmlData(element, "techspecs"),
                StandardEquipment = ReadStringXmlData(element, "standardequipment"),
                OptionalEquipment = ReadStringXmlData(element, "optionalequipment"),
                ModelVideo = ReadStringXmlData(element, "modelvideo"),
                VideoFeedId = ReadIntXmlData(element, "videofeedid"),
                VideoFeedUrl = ReadStringXmlData(element, "videofeedurl"),
                Extended = ReadStringXmlData(element, "extended"),
                Vin = ReadStringXmlData(element, "vin"),
                DmsId = ReadIntXmlData(element, "dmsid") ?? 0
              };
            db.Save(inventoryimport);
            db.Flush();
          }

          #endregion

          #region tinventoryimportlock table

          //TODO: add this when the lock table becomes used in the system

          #endregion

          #region tinventoryimportmapping table

          foreach (XElement element in document.Descendants("tinventoryimportmapping"))
          {
            var inventoryimportmapping = new InventoryImportMapping
              {
                InventoryImportMappingId = ReadIntXmlData(element, "inventoryimportmappingid") ?? 0,
                DmsId = ReadIntXmlData(element, "dmsid") ?? 0,
                DestinationField = ReadStringXmlData(element, "destinationfield"),
                SourceField = ReadStringXmlData(element, "sourcefield"),
                WhereClause = ReadStringXmlData(element, "whereclause")
              };
            db.Save(inventoryimportmapping);
            db.Flush();
          }

          #endregion

          #region tinventoryoverride table

          foreach (XElement element in document.Descendants("tinventoryoverride"))
          {
            var inventoryoverride = new InventoryOverride
              {
                DmsId = ReadIntXmlData(element, "dmsid") ?? 0,
                VIN = ReadStringXmlData(element, "vin"),
                FeatureChoice = ReadIntXmlData(element, "featurechoice") ?? 0,
                Include = ReadBoolXmlData(element, "include")
              };
            db.Save(inventoryoverride);
            db.Flush();
          }

          #endregion

          #region tinventoryraw table

          foreach (XElement element in document.Descendants("tinventoryraw"))
          {
            var inventoryoverride = new InventoryRaw
              {
                ImportSourceId = ReadIntXmlData(element, "importsourceid") ?? 0,
                DmsId = ReadIntXmlData(element, "dmsid") ?? 0,
                Vin = ReadStringXmlData(element, "vin"),
                VehicleId = ReadIntXmlData(element, "vehicleid"),
                DealerId = ReadStringXmlData(element, "dealerid"),
                LocationId = ReadIntXmlData(element, "locationid"),
                StockNumber = ReadStringXmlData(element, "stocknumber"),
                VehicleType = ReadStringXmlData(element, "vehicletype"),
                VehicleYear = ReadIntXmlData(element, "vehicleyear"),
                VehicleMake = ReadStringXmlData(element, "vehiclemake"),
                VehicleModel = ReadStringXmlData(element, "vehiclemodel"),
                VehicleModelNumber = ReadStringXmlData(element, "vehiclemodelnumber"),
                VehicleTrim = ReadStringXmlData(element, "vehicletrim"),
                VehicleStyleDescription = ReadStringXmlData(element, "vehiclestyledescription"),
                FriendlyStyle = ReadStringXmlData(element, "friendlystyle"),
                Mileage = ReadIntXmlData(element, "mileage"),
                Certified = ReadBoolXmlData(element, "certified"),
                Description1 = ReadStringXmlData(element, "description1"),
                Description2 = ReadStringXmlData(element, "description2"),
                Options = ReadStringXmlData(element, "options"),
                DateInStock = ReadDateXmlData(element, "dateinstock"),
                InternetPrice = ReadIntXmlData(element, "internetprice"),
                Msrp = ReadIntXmlData(element, "msrp"),
                BookValue = ReadIntXmlData(element, "bookvalue"),
                IsSpecial = ReadBoolXmlData(element, "isspecial"),
                IsHomeSpecial = ReadBoolXmlData(element, "ishomespecial"),
                SpecialPrice = ReadIntXmlData(element, "specialprice"),
                SpecialStartDate = ReadDateXmlData(element, "specialstartdate"),
                SpecialEndDate = ReadDateXmlData(element, "specialenddate"),
                SpecialMonthlyPayment = ReadIntXmlData(element, "specialmonthlypayment"),
                SpecialDisclaimer = ReadStringXmlData(element, "specialdisclaimer"),
                Invoice = ReadIntXmlData(element, "invoice"),
                SellingPrice = ReadIntXmlData(element, "sellingprice"),
                OtherPrice1 = ReadIntXmlData(element, "otherprice1"),
                OtherPrice2 = ReadIntXmlData(element, "otherprice2"),
                OtherPrice3 = ReadIntXmlData(element, "otherprice3"),
                OtherPrice4 = ReadIntXmlData(element, "otherprice4"),
                OtherPrice5 = ReadIntXmlData(element, "otherprice5"),
                OtherPrice6 = ReadIntXmlData(element, "otherprice6"),
                OtherPrice7 = ReadIntXmlData(element, "otherprice7"),
                BodyType = ReadStringXmlData(element, "bodytype"),
                NumberDoors = ReadStringXmlData(element, "numberdoors"),
                ExtColor = ReadStringXmlData(element, "extcolor"),
                ExtColorGeneric = ReadStringXmlData(element, "extcolorgeneric"),
                ExtColorCode = ReadStringXmlData(element, "extcolorcode"),
                IntColor = ReadStringXmlData(element, "intcolor"),
                IntColorGeneric = ReadStringXmlData(element, "intcolorgeneric"),
                IntColorCode = ReadStringXmlData(element, "intcolorcode"),
                ExtColorHexcode = ReadStringXmlData(element, "extcolorhexcode"),
                IntColorHexcode = ReadStringXmlData(element, "intcolorhexcode"),
                VehicleUpholstery = ReadStringXmlData(element, "vehicleupholstery"),
                EngineCylinders = ReadStringXmlData(element, "enginecylinders"),
                EngineLiters = ReadStringXmlData(element, "engineliters"),
                EngineBlock = ReadStringXmlData(element, "engineblock"),
                EngineAspiration = ReadStringXmlData(element, "engineaspiration"),
                EngineDescription = ReadStringXmlData(element, "enginedescription"),
                EngineCubicInches = ReadStringXmlData(element, "enginecubicinches"),
                Transmission = ReadStringXmlData(element, "transmission"),
                TransmissionSpeed = ReadIntXmlData(element, "transmissionspeed"),
                TransmissionDescription = ReadStringXmlData(element, "transmissiondescription"),
                DriveTrain = ReadStringXmlData(element, "drivetrain"),
                FuelType = ReadStringXmlData(element, "fueltype"),
                EpaCity = ReadStringXmlData(element, "epacity"),
                EpaHighway = ReadStringXmlData(element, "epahighway"),
                EpaClassification = ReadStringXmlData(element, "epaclassification"),
                WheelBase = ReadStringXmlData(element, "wheelbase"),
                PassengerCapacity = ReadStringXmlData(element, "passengercapacity"),
                MarketClass = ReadStringXmlData(element, "marketclass"),
                Comments1 = ReadStringXmlData(element, "comments1"),
                Comments2 = ReadStringXmlData(element, "comments2"),
                Comments3 = ReadStringXmlData(element, "comments3"),
                Comments4 = ReadStringXmlData(element, "comments4"),
                Comments5 = ReadStringXmlData(element, "comments5"),
                IsSold = ReadBoolXmlData(element, "issold"),
                IsUpdated = ReadBoolXmlData(element, "isupdated"),
                ImagesModifiedDate = ReadDateXmlData(element, "imagesmodifieddate"),
                DataModifiedDate = ReadDateXmlData(element, "datamodifieddate"),
                DateCreated = ReadDateXmlData(element, "datecreated"),
                TechSpecs = ReadStringXmlData(element, "techspecs"),
                ExteriorEquipment = ReadStringXmlData(element, "exteriorequipment"),
                StandardEquipment = ReadStringXmlData(element, "standardequipment"),
                InteriorEquipment = ReadStringXmlData(element, "interiorequipment"),
                MechanicalEquipment = ReadStringXmlData(element, "mechanicalequipment"),
                SafetyEquipment = ReadStringXmlData(element, "safetyequipment"),
                OptionalEquipment = ReadStringXmlData(element, "optionalequipment"),
                OptionalEquipmentShort = ReadStringXmlData(element, "optionalequipmentshort"),
                VideoLink = ReadStringXmlData(element, "videolink"),
                MediaLink = ReadStringXmlData(element, "medialink"),
                StockImage = ReadStringXmlData(element, "stockimage"),
                ImageList = ReadStringXmlData(element, "imagelist"),
                StandardMake = ReadStringXmlData(element, "standardmake"),
                StandardModel = ReadStringXmlData(element, "standardmodel"),
                StandardBody = ReadStringXmlData(element, "standardbody"),
                StandardTrim = ReadStringXmlData(element, "standardtrim"),
                StandardYear = ReadStringXmlData(element, "standardyear"),
                StandardStyle = ReadStringXmlData(element, "standardstyle"),
                DaysInStock = ReadIntXmlData(element, "daysinstock"),
                ImageCount = ReadIntXmlData(element, "imagecount"),
                DealerName = ReadStringXmlData(element, "dealername"),
                DealerAddress = ReadStringXmlData(element, "dealeraddress"),
                DealerCity = ReadStringXmlData(element, "dealercity"),
                DealerState = ReadStringXmlData(element, "dealerstate"),
                DealerZip = ReadStringXmlData(element, "dealerzip"),
                DealerContact = ReadStringXmlData(element, "dealercontact"),
                DealerPhone = ReadStringXmlData(element, "dealerphone"),
                DealerEmail = ReadStringXmlData(element, "dealeremail"),
                DealerUrl = ReadStringXmlData(element, "dealerurl"),
                VehicleTitle = ReadStringXmlData(element, "vehicletitle"),
                OtherVif = ReadStringXmlData(element, "othervif"),
                OtherSend = ReadStringXmlData(element, "othersend"),
                OtherColor1 = ReadStringXmlData(element, "othercolor1"),
                OtherColor2 = ReadStringXmlData(element, "othercolor2"),
                OtherColor3 = ReadStringXmlData(element, "othercolor3"),
                CarfaxOneOwner = ReadBoolXmlData(element, "carfaxoneowner"),
                CarfaxHistoryUrl = ReadStringXmlData(element, "carfaxhistoryurl"),
                VehiclesTestVideo = ReadStringXmlData(element, "vehiclestestvideo"),
                PackageCode = ReadStringXmlData(element, "packagecode"),
                JobRunTime = ReadDateXmlData(element, "jobruntime") ?? DateTime.Now,
                Other = ReadStringXmlData(element, "other"),
                Category = ReadStringXmlData(element, "category")
              };
            db.Save(inventoryoverride);
            db.Flush();
          }

          #endregion

          #region VehiclePackages table

          foreach (XElement element in document.Descendants("vehiclepackages"))
          {
            var vehiclepackages = new VehiclePackages
              {
                country = ReadIntXmlData(element,"country") ?? 0,
                vehicle_id = ReadLongXmlData(element,"vehicle_id") ?? 0,
                PackageCode = ReadStringXmlData(element,"packagecode"),
                OemName = ReadStringXmlData(element,"oemname"),
                MSRP = ReadFloatXmlData(element,"msrp"),
                Name1 = ReadStringXmlData(element,"name1"),
                Name2 = ReadStringXmlData(element,"name2"),
                Contains1 = ReadStringXmlData(element,"contains1"),
                Contains2 = ReadStringXmlData(element,"contains2")
              };
            db.Save(vehiclepackages);
            db.Flush();
          }
          #endregion

          #region VehicleOptions table

          foreach (XElement element in document.Descendants("vehicleoptions"))
          {
            var vehicleoptions = new VehicleOptions
              {
                country = ReadIntXmlData(element, "country") ?? 0,
                vehicle_id = ReadLongXmlData(element, "vehicle_id") ?? 0,
                OptionCode = ReadStringXmlData(element, "optioncode"),
                MSRP = ReadFloatXmlData(element, "msrp"),
                OemName = ReadStringXmlData(element, "oemname"),
                Name1 = ReadStringXmlData(element, "name1"),
                Name2 = ReadStringXmlData(element, "name2")
              };
            db.Save(vehicleoptions);
            db.Flush();
          }

          #endregion
          #region vehicle table

          foreach (XElement element in document.Descendants("vehicles"))
          {
            var vehicles = new Vehicles
              {
                country = ReadIntXmlData(element,"country") ?? 0,
                vehicle_id = ReadLongXmlData(element,"vehicle_id") ?? 0,
                id_101 = ReadIntXmlData(element,"id_101"),
                body1 = ReadStringXmlData(element,"body1"),
                body2 = ReadStringXmlData(element,"body2"),
                cylinders = ReadStringXmlData(element,"cylinders"),
                doors = ReadStringXmlData(element,"doors"),
                drive_type1 = ReadStringXmlData(element,"drive_type1"),
                drive_type2 = ReadStringXmlData(element,"drive_type2"),
                engine = ReadStringXmlData(element,"engine"),
                photo1 = ReadStringXmlData(element,"photo1"),
                photo2 = ReadStringXmlData(element, "photo2"),
                photo3 = ReadStringXmlData(element, "photo3"),
                ext360 = ReadStringXmlData(element,"ext360"),
                msrp = ReadStringXmlData(element,"msrp"),
                mpg_city = ReadStringXmlData(element,"mpg_city"),
                mpg_hwy = ReadStringXmlData(element,"mpg_hwy"),
                transmission1 = ReadStringXmlData(element,"transmission1"),
                transmission2 = ReadStringXmlData(element,"transmission2"),
                seating = ReadStringXmlData(element,"seating"),
                trim = ReadStringXmlData(element,"trim"),
                trimclass = ReadStringXmlData(element,"trimclass"),
                engine_config1 = ReadStringXmlData(element,"engine_config1"),
                engine_config2 = ReadStringXmlData(element, "engine_config2"),
                fuel_type1 = ReadStringXmlData(element,"fuel_type1"),
                fuel_type2 = ReadStringXmlData(element, "fuel_type2"),
                int360 = ReadStringXmlData(element,"int360"),
                make = ReadStringXmlData(element,"make"),
                model = ReadStringXmlData(element,"model"),
                modelcode = ReadStringXmlData(element,"modelcode"),
                year = ReadIntXmlData(element,"year"),
              };
            db.Save(vehicles);
            db.Flush();
          }
          #endregion
        }
      }
    }

    private static DateTime? ReadDateXmlData(XElement element, string fieldName)
    {
      if (element.Element(fieldName) != null)
      {
        string tempValue = element.Element(fieldName).Value;
        DateTime tempDateValue;
        if (DateTime.TryParse(tempValue, out tempDateValue))
        {
          return tempDateValue;
        }
      }
      return null;
    }

    private static long? ReadLongXmlData(XElement element, string fieldName)
    {
      if (element.Element(fieldName) != null)
      {
        string tempValue = element.Element(fieldName).Value;
        long tempIntValue;
        if (long.TryParse(tempValue, out tempIntValue))
        {
          return tempIntValue;
        }
      }
      return null;
    }

    private static int? ReadIntXmlData(XElement element, string fieldName)
    {
      if (element.Element(fieldName) != null)
      {
        string tempValue = element.Element(fieldName).Value;
        int tempIntValue;
        if (int.TryParse(tempValue, out tempIntValue))
        {
          return tempIntValue;
        }
      }
      return null;
    }

    private static float ReadFloatXmlData(XElement element, string fieldName)
    {
      float tempFloatValue;
      if (element.Element(fieldName) != null)
      {
        string tempValue = element.Element(fieldName).Value;

        if (float.TryParse(tempValue, out tempFloatValue))
        {
          return tempFloatValue;
        }
      }
      tempFloatValue = 0;
      return tempFloatValue;
    }


    private static bool? ReadBoolXmlData(XElement element, string fieldName)
    {
      if (element.Element(fieldName) != null)
      {
        string tempValue = element.Element(fieldName).Value;
        bool tempBoolValue;
        if (tempValue == "1")
        {
          return true;
        }
        else if (tempValue == "0")
        {
          return false;
        }
        else if (bool.TryParse(tempValue, out tempBoolValue))
        {
          return tempBoolValue;
        }
      }
      return null;
    }

    private static string ReadStringXmlData(XElement element, string fieldName)
    {
      if (element.Element(fieldName) != null)
      {
        return element.Element(fieldName).Value;
      }
      else
      {
        return "";
      }
    }
  }
}

*/