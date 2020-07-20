using System.Collections.Generic;

namespace redseat_server.Downloaders.AllDebrid
{
    public class AllDebridLink    {
        public AllDebridLink() {}
        public string Link { get; set; } 
        public string Filename { get; set; } 
        public int Size { get; set; } 

        public List<string> Files { get; set; } 

    }
}