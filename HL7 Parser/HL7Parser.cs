using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7_Parser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class HL7Parser
    {
        private List<string> segmentList = null; // Data Segment Listesi

        // HL7 Verisini alır
        public HL7Parser(string hlData)
        {
            this.segmentList = DataParse(hlData);
        }

        // Segmentlere ayır
        private List<string> DataParse(string hlData)
        {
            hlData = DataClean(hlData);
            var segments = new List<string>(hlData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            return segments;
        }

        // İstenmeyen karakterleri temizle
        private string DataClean(string hlData)
        {
            string[] unwantedChars = { "\u000b", "\u001c", "." };
            foreach (var character in unwantedChars)
            {
                hlData = hlData.Replace(character, string.Empty);
            }
            return hlData;
        }

        // Belirli iki karakter arasındaki veriyi al
        private string SubStrSpecial(string start, string end, string strData)
        {
            int startPos = strData.IndexOf(start);
            if (startPos == -1)
            {
                return null;
            }
            startPos += start.Length;
            int endPos = strData.IndexOf(end, startPos);
            if (endPos == -1)
            {
                return null;
            }
            return strData.Substring(startPos, endPos - startPos);
        }

        // -> | <- Ayracı Tanımlama
        private string SepDetector(string data, string separator = "|", int index = 0)
        {
            var segments = data.Split(new[] { separator }, StringSplitOptions.None);
            return segments.Length > index ? segments[index] : null;
        }

        // Segment analizini yap ve Dictionary döndür
        public Dictionary<string, string> SegmentAnalyze()
        {
            if (!segmentList[0].StartsWith("MSH"))
            {
                throw new Exception("Hatalı HL7 Verisi");
            }

            // Cihaz tarihini al ve formatla
            DateTime? cihazTarih = null;
            string cihazTarihStr = SepDetector(segmentList[0], "|", 6);
            if (DateTime.TryParseExact(cihazTarihStr, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                cihazTarih = parsedDate;
            }

            // Hasta verisini dictionary olarak doldur
            var result = new Dictionary<string, string>
        {
            { "IsimSoyisim", SepDetector(segmentList[1], "|", 5) },
            { "CihazTarih", cihazTarih?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Geçersiz tarih" }, // Formatlanmış tarih
            { "HastaOda", SepDetector(segmentList[2], "|", 3) },
            { "HastaCihaz", SubStrSpecial("|^~\\&|^", "|", segmentList[0]) },
            { "HastaProtokol", SepDetector(segmentList[2], "|", 19) },
            { "HastaSpo2", SepDetector(segmentList[5], "|", 5) },
            { "HastaPerf", SepDetector(segmentList[6], "|", 5) },
            { "HastaNabiz", segmentList.Count > 7 ? SepDetector(segmentList[7], "|", 5) : "Veri yok" },
            { "HastaSistolik", segmentList.Count > 8 ? SepDetector(segmentList[8], "|", 5) : "Veri yok" },
            { "HastaDiyastolik", segmentList.Count > 9 ? SepDetector(segmentList[9], "|", 5) : "Veri yok" },
            { "HastaNabizOrt", segmentList.Count > 10 ? SepDetector(segmentList[10], "|", 5) : "Veri yok" }
        };

            return result;
        }
    }

}
