using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using redseat_server.Models;
using Microsoft.EntityFrameworkCore;
using redseat_server.Services;

namespace redseat_server.Controllers
{
    public class BrowseResult {
        public string Parent {get;set;}
        public IEnumerable<string> Children {get;set;}
    }
    [ApiController]
    [Route("[controller]")]
    public class BrowseController : ControllerBase
    {
        private readonly ILogger<BrowseController> _logger;
        private readonly IIoService _ioService;

        public BrowseController(ILogger<BrowseController> logger, IIoService ioService)
        {
            _logger = logger;
            _ioService = ioService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<String>> Get(string path)
        {
            if (path == null) path = _ioService.DefaultPath;
            return Ok(new BrowseResult(){ Parent = path, Children =_ioService.GetFolders(path)});
        }

    }
}
