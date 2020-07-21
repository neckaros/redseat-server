using System.Collections.Generic;

namespace RedSeatServer.Downloaders.AllDebrid
{
    public class AllDebridLink    {
        public AllDebridLink() {}
        public string Link { get; set; } 
        public string Filename { get; set; } 
        public int Size { get; set; } 

        public List<string> Files { get; set; } 

    }
}