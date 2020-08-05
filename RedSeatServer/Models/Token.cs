using AutoMapper;

namespace RedSeatServer.Models {

    
    class TokenMapperProfile : Profile {
        public TokenMapperProfile()
        {
            CreateMap<Token, TokenDto>();
        }
    }
    public class Token {
        public string Name {get; set;}
        public string Id {get; set;}
        public string Key {get; set;}
    }

    
    public class TokenDto {
      
        public string Name {get; set;}
    }
}