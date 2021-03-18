using DrovoAPI.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DrovoAPI.Classes
{
    public class SiteView
    {
        public virtual int Id { get; set; }
        public virtual string Domain { get; set; }
        public virtual int[] CategoryIds { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Image { get; set; }

        public virtual string SearchString { get; set; }
        public virtual int ItemsPerPage { get; set; }

        public virtual Task<List<Product>> Search(string searchQuery)
        {
            throw new NotImplementedException();
        }
        public virtual async Task<string> ReadHtml(string url, Encoding encoding)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers = new WebHeaderCollection {
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36" },
                 {"Accept","*/*" },
                    { "Connection","keep-alive" },
                    {"Content-type","charset=UTF-8" },
                    {"Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" }
            };
                request.Timeout = 15000;
                WebResponse webResponse = await request.GetResponseAsync();

                HttpWebResponse response = (HttpWebResponse)(webResponse);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (String.IsNullOrWhiteSpace(response.CharacterSet))
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, encoding);

                    string data = await readStream.ReadToEndAsync();

                    response.Close();
                    readStream.Close();

                    return data;
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static Encoding GetEncoding(Stream file)
        {
            // Read the BOM
            var bom = new byte[4];

            Stream temp = file;

            temp.Read(bom, 0, 4);

            file.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }

        public virtual Task<HtmlNodeCollection> GetProducts(int page, string searchQuery)
        {
            throw new NotImplementedException();
        }

        public virtual string GetTitle(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public virtual string GetDescription(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public virtual string GetPrice(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public virtual string GetImage(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public virtual string GetRating(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public virtual string GetPopularity(HtmlNode item)
        {
            throw new NotImplementedException();
        }
        public virtual string GetLink(HtmlNode item)
        {
            throw new NotImplementedException();
        }
    }
}
