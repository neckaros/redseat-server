using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RedSeatServer.Services.Parser;

namespace RedSeatServer.Extensions
{
    public static class MediaFileExtensions
    {
        private static Dictionary<string, Quality> _fileExtensions;

        static MediaFileExtensions()
        {
            _fileExtensions = new Dictionary<string, Quality>(StringComparer.OrdinalIgnoreCase)
            {
                //Unknown
                { ".webm", Quality.Unknown },

                //SDTV
                { ".m4v", Quality.SDTV },
                { ".3gp", Quality.SDTV },
                { ".nsv", Quality.SDTV },
                { ".ty", Quality.SDTV },
                { ".strm", Quality.SDTV },
                { ".rm", Quality.SDTV },
                { ".rmvb", Quality.SDTV },
                { ".m3u", Quality.SDTV },
                { ".ifo", Quality.SDTV },
                { ".mov", Quality.SDTV },
                { ".qt", Quality.SDTV },
                { ".divx", Quality.SDTV },
                { ".xvid", Quality.SDTV },
                { ".bivx", Quality.SDTV },
                { ".nrg", Quality.SDTV },
                { ".pva", Quality.SDTV },
                { ".wmv", Quality.SDTV },
                { ".asf", Quality.SDTV },
                { ".asx", Quality.SDTV },
                { ".ogm", Quality.SDTV },
                { ".ogv", Quality.SDTV },
                { ".m2v", Quality.SDTV },
                { ".avi", Quality.SDTV },
                { ".bin", Quality.SDTV },
                { ".dat", Quality.SDTV },
                { ".dvr-ms", Quality.SDTV },
                { ".mpg", Quality.SDTV },
                { ".mpeg", Quality.SDTV },
                { ".mp4", Quality.SDTV },
                { ".avc", Quality.SDTV },
                { ".vp3", Quality.SDTV },
                { ".svq3", Quality.SDTV },
                { ".nuv", Quality.SDTV },
                { ".viv", Quality.SDTV },
                { ".dv", Quality.SDTV },
                { ".fli", Quality.SDTV },
                { ".flv", Quality.SDTV },
                { ".wpl", Quality.SDTV },

                //DVD
                { ".img", Quality.DVD },
                { ".iso", Quality.DVD },
                { ".vob", Quality.DVD },

                //HD
                { ".mkv", Quality.HDTV720p },
                { ".ts", Quality.HDTV720p },
                { ".wtv", Quality.HDTV720p },

                //Bluray
                { ".m2ts", Quality.Bluray720p }
            };
        }

        public static HashSet<string> Extensions => new HashSet<string>(_fileExtensions.Keys, StringComparer.OrdinalIgnoreCase);

        public static Quality GetQualityForExtension(string extension)
        {
            if (_fileExtensions.ContainsKey(extension))
            {
                return _fileExtensions[extension];
            }

            return Quality.Unknown;
        }
        private static readonly RegexReplace NormalizeRegex = new RegexReplace(@"((?:\b|_)(?<!^)(a(?!$)|an|the|and|or|of)(?:\b|_))|\W|_",
                                                                string.Empty,
                                                                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex PercentRegex = new Regex(@"(?<=\b\d+)%", RegexOptions.Compiled);
        public static string CleanSeriesTitle(this string title)
        {
            long number = 0;

            //If Title only contains numbers return it as is.
            if (long.TryParse(title, out number))
                return title;

            // Replace `%` with `percent` to deal with the 3% case
            title = PercentRegex.Replace(title, "percent");

            return NormalizeRegex.Replace(title).ToLower().RemoveAccent();
        }
    }
}