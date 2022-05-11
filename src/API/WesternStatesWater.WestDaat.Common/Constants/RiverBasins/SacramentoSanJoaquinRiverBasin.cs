﻿using GeoJSON.Text.Feature;
using GeoJSON.Text.Geometry;

namespace WesternStatesWater.WestDaat.Common.Constants.RiverBasins
{
    public static class SacramentoSanJoaquinRiverBasin
    {
        public const string BasinName = "Sacramento - San Joaquin River Basin";

        public static Feature Feature
        {
            get
            {
                return new Feature
                {
                    Properties = new Dictionary<string, Object>
                    {
                        { "BasinName", BasinName },
                        { "Shape_Leng", 28.373090939 },
                        { "Shape_Area", 16.0397673794 },
                    },
                    Geometry = new Polygon(new List<LineString>
                    {
                        new LineString(new List<IPosition>
                        {
new Position(longitude:-120.09449806999999,latitude:39.586227414000064),
new Position(longitude:-120.14370783199996,latitude:39.720370202000026),
new Position(longitude:-120.10083053399995,latitude:39.90041671100005),
new Position(longitude:-120.21540937399999,latitude:40.06634186500003),
new Position(longitude:-120.40552755199997,latitude:40.14357872000005),
new Position(longitude:-120.58422698099997,latitude:40.28646616100008),
new Position(longitude:-120.760399184,latitude:40.318913319000046),
new Position(longitude:-121.24642614099997,latitude:40.529168682000034),
new Position(longitude:-121.05974646399994,latitude:40.616176188000054),
new Position(longitude:-121.11505894699997,latitude:40.70278159600008),
new Position(longitude:-120.83133698699999,latitude:40.84285740100006),
new Position(longitude:-120.829985852,latitude:40.90782560900004),
new Position(longitude:-120.67162350299998,latitude:40.95153417500006),
new Position(longitude:-120.57342235399994,latitude:41.061766695000074),
new Position(longitude:-120.44001952599996,latitude:41.11943194600008),
new Position(longitude:-120.37021585899998,latitude:41.01447387700006),
new Position(longitude:-120.21024965299995,latitude:41.02566906000004),
new Position(longitude:-120.13374322899995,latitude:41.19197080400005),
new Position(longitude:-120.18896617899998,latitude:41.26010866400003),
new Position(longitude:-120.21985114599994,latitude:41.417148990000044),
new Position(longitude:-120.28365012799998,latitude:41.53257021100006),
new Position(longitude:-120.29107728399998,latitude:41.65849304500006),
new Position(longitude:-120.202712685,latitude:41.82814573600007),
new Position(longitude:-120.25448665299996,latitude:41.911084997000046),
new Position(longitude:-120.19346654599997,latitude:41.97259895800005),
new Position(longitude:-120.23520291899996,latitude:42.117193033000035),
new Position(longitude:-120.37554233099996,latitude:42.39030372000008),
new Position(longitude:-120.468278511,latitude:42.41162912300007),
new Position(longitude:-120.68835540499998,latitude:42.34959302000004),
new Position(longitude:-120.82602370099994,latitude:42.32626006500004),
new Position(longitude:-120.82874733299997,latitude:42.13718189100007),
new Position(longitude:-120.69936899799995,latitude:41.993885542000044),
new Position(longitude:-120.55490060299996,latitude:41.92634819300008),
new Position(longitude:-120.63081376499997,latitude:41.771106632000055),
new Position(longitude:-120.80835714899996,latitude:41.68328464600006),
new Position(longitude:-120.86644209599996,latitude:41.55191188100008),
new Position(longitude:-120.96442226999994,latitude:41.60306871700004),
new Position(longitude:-121.14927822899995,latitude:41.44123229000007),
new Position(longitude:-121.27921324899995,latitude:41.62530892700005),
new Position(longitude:-121.50765162899995,latitude:41.57653484100007),
new Position(longitude:-121.62031032799996,latitude:41.61327358900007),
new Position(longitude:-121.68253086399994,latitude:41.572224799000026),
new Position(longitude:-122.05110855999999,latitude:41.44900205700003),
new Position(longitude:-122.13012719799997,latitude:41.504589054000064),
new Position(longitude:-122.19148722899996,latitude:41.41278100000005),
new Position(longitude:-122.39252871699995,latitude:41.36865168600008),
new Position(longitude:-122.49915014699997,latitude:41.312031869000066),
new Position(longitude:-122.514058122,latitude:41.22310178200007),
new Position(longitude:-122.44602292199994,latitude:41.12981345000003),
new Position(longitude:-122.54189392599994,latitude:41.066766366000024),
new Position(longitude:-122.60391754999995,latitude:40.88932373000006),
new Position(longitude:-122.71673910499999,latitude:40.70296938200005),
new Position(longitude:-122.70325550399997,latitude:40.59494882400003),
new Position(longitude:-122.79076664299998,latitude:40.51351736500004),
new Position(longitude:-122.99862760799999,latitude:40.42975428300008),
new Position(longitude:-123.06833222499995,latitude:40.334697783000024),
new Position(longitude:-122.99832842399996,latitude:40.25678148600008),
new Position(longitude:-122.98769331299997,latitude:40.144271755000034),
new Position(longitude:-122.91689609099996,latitude:39.929165770000054),
new Position(longitude:-122.92539674599999,latitude:39.78142263800004),
new Position(longitude:-122.67403544599995,latitude:39.533582625000065),
new Position(longitude:-122.78713173299997,latitude:39.406212793000066),
new Position(longitude:-122.72701718999997,latitude:39.28926566300004),
new Position(longitude:-122.85728816599999,latitude:39.24909256300003),
new Position(longitude:-122.93487077199995,latitude:39.30145662000007),
new Position(longitude:-123.08834142599994,latitude:39.089221846000044),
new Position(longitude:-122.94726511399995,latitude:38.91133655300007),
new Position(longitude:-122.81766802199996,latitude:38.85195608200007),
new Position(longitude:-122.69949906599999,latitude:38.71940068400005),
new Position(longitude:-122.40785649799994,latitude:38.54973140300007),
new Position(longitude:-122.28349780599996,latitude:38.552660880000076),
new Position(longitude:-122.21515718699999,latitude:38.39998235200005),
new Position(longitude:-122.11596756999995,latitude:38.41957456800003),
new Position(longitude:-122.05731191599995,latitude:38.32850282800007),
new Position(longitude:-121.88845865599995,latitude:38.29811842200007),
new Position(longitude:-121.80545050399996,latitude:38.13348754500004),
new Position(longitude:-121.84536934699997,latitude:37.99791034200007),
new Position(longitude:-121.92694138599995,latitude:37.86543646100006),
new Position(longitude:-121.66249099299995,latitude:37.75237730500004),
new Position(longitude:-121.40264749999994,latitude:37.31506969000003),
new Position(longitude:-121.40660099599995,latitude:37.16186881900006),
new Position(longitude:-121.23099036399998,latitude:37.157440878000045),
new Position(longitude:-121.22179617499995,latitude:36.91444519200007),
new Position(longitude:-121.10854771199996,latitude:36.76863928100005),
new Position(longitude:-120.96427150099998,latitude:36.720013409000046),
new Position(longitude:-120.99107287599998,latitude:36.52732642500007),
new Position(longitude:-120.85952359799995,latitude:36.48587554300008),
new Position(longitude:-120.77165046699997,latitude:36.40325925600007),
new Position(longitude:-120.65169983799996,latitude:36.380244805000075),
new Position(longitude:-120.66799588299995,latitude:36.14194000100008),
new Position(longitude:-120.43728407499998,latitude:35.97308739700003),
new Position(longitude:-120.29486032599999,latitude:35.93654970400007),
new Position(longitude:-120.19297021999995,latitude:35.75191872700003),
new Position(longitude:-120.22448687399998,latitude:35.63892389200004),
new Position(longitude:-120.089255719,latitude:35.502227918000074),
new Position(longitude:-119.91890172699999,latitude:35.401514804000044),
new Position(longitude:-119.55093609999994,latitude:35.088967524000054),
new Position(longitude:-119.533334476,latitude:34.99407694200005),
new Position(longitude:-119.35389684299997,latitude:34.87568661700004),
new Position(longitude:-119.22922321499999,latitude:34.85378557300004),
new Position(longitude:-118.99144476799995,latitude:34.767404430000056),
new Position(longitude:-118.82855975599995,latitude:34.803724405000025),
new Position(longitude:-118.72315154399996,latitude:34.89027765800006),
new Position(longitude:-118.53394252499999,latitude:34.98600028900006),
new Position(longitude:-118.41158467899999,latitude:35.107967020000046),
new Position(longitude:-118.42035027399999,latitude:35.203806360000044),
new Position(longitude:-118.246289806,latitude:35.22605655900003),
new Position(longitude:-118.29531900399996,latitude:35.37885289900004),
new Position(longitude:-118.18220900999995,latitude:35.429085265000026),
new Position(longitude:-118.12047294899997,latitude:35.61120814100008),
new Position(longitude:-117.98910183199996,latitude:35.69862466700005),
new Position(longitude:-117.98751434899998,latitude:35.92044373500005),
new Position(longitude:-118.11224075699994,latitude:36.215111182000044),
new Position(longitude:-118.10915482799999,latitude:36.33656945200005),
new Position(longitude:-118.21210389499998,latitude:36.43242726200003),
new Position(longitude:-118.25179350699995,latitude:36.54507837800003),
new Position(longitude:-118.34169254099999,latitude:36.654254123000044),
new Position(longitude:-118.36342772599994,latitude:36.875951462000046),
new Position(longitude:-118.44103683399999,latitude:37.06454392400008),
new Position(longitude:-118.66107927699994,latitude:37.15504829900004),
new Position(longitude:-118.67678387899997,latitude:37.283505038000044),
new Position(longitude:-118.78543514899997,latitude:37.34760189000008),
new Position(longitude:-118.76697629899996,latitude:37.43992471100006),
new Position(longitude:-118.91415211499998,latitude:37.55519590600005),
new Position(longitude:-119.01513407399995,latitude:37.57929199800003),
new Position(longitude:-119.06450634699996,latitude:37.68795393500005),
new Position(longitude:-119.20094311399998,latitude:37.73725148500006),
new Position(longitude:-119.20789076999995,latitude:37.894102291000024),
new Position(longitude:-119.30454291899997,latitude:37.94208121500003),
new Position(longitude:-119.33581986399997,latitude:38.07799300500005),
new Position(longitude:-119.57182371199997,latitude:38.157558668000036),
new Position(longitude:-119.62366585199999,latitude:38.19907843400006),
new Position(longitude:-119.63322878799994,latitude:38.35225942000005),
new Position(longitude:-119.85099825699996,latitude:38.600703543000066),
new Position(longitude:-120.05541249399994,latitude:38.74821466900005),
new Position(longitude:-120.15292272699998,latitude:38.870296481000025),
new Position(longitude:-120.26191784499997,latitude:39.22094343900005),
new Position(longitude:-120.37346674399998,latitude:39.42926453700005),
new Position(longitude:-120.29975078599995,latitude:39.50387180100006),
new Position(longitude:-120.09449806999999,latitude:39.586227414000064)
                        })
                    })
                };
            }
        }
    }
}