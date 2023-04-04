using System;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace PesoADollar
{
    class Serie
    {
        [DataMember(Name = "titulo")]
        public string Title { get; set; }

        [DataMember(Name = "idSerie")]
        public string IdSerie { get; set; }

        [DataMember(Name = "datos")]
        public DataSerie[] Data { get; set; }

    }

    [DataContract]
    class DataSerie
    {
        [DataMember(Name = "fecha")]
        public string Date { get; set; }

        [DataMember(Name = "dato")]
        public string Data { get; set; }
    }

    [DataContract]
    class SeriesResponse
    {
        [DataMember(Name = "series")]
        public Serie[] series { get; set; }
    }

    [DataContract]
    class Response
    {
        [DataMember(Name = "bmx")]
        public SeriesResponse seriesResponse { get; set; }
    }

    class Consulta
    {
        public static Response ReadSerie()
        {
            try
            {
                string url = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF63528/datos/oportuno/";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Accept = "application/json";
                request.Headers["Bmx-Token"] = "81775333cca91e0ea3fa092d3ecf17e29643b3a3a5f88134a11c85f300e9fb1c";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                Response jsonResponse = objResponse as Response;
                return jsonResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        private void Main(string[] args)
        {
            Response response = ReadSerie();
            Serie serie = response.seriesResponse.series[0];
            Console.WriteLine("Serie: {0}", serie.Title);
            foreach (DataSerie dataSerie in serie.Data) {
                if (dataSerie.Data.Equals("N/E")) continue;
                Console.WriteLine("Fecha: {0}", dataSerie.Date);
                Console.WriteLine("Dato: {0}", dataSerie.Data);
            }
            Console.ReadLine();
        }
    }
}