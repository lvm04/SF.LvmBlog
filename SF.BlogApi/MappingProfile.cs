using AutoMapper;
using SF.BlogApi.Contracts;
using SF.BlogData;
using SF.BlogData.Models;

namespace SF.BlogApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        CreateMap<User, UserView>().ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name)));
        CreateMap<CreateUserRequest, User>();

        CreateMap<Comment, CommentView>();
        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<EditCommentRequest, Comment>();

        CreateMap<Article, ArticleView>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => $"{src.Author.Name} (ID: {src.Author.Id}, Login: {src.Author.Login})"))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Select(t => t.Text)));
    }
}
