using System;

namespace Application.Comments {
    //Purpose of DTO's for Comments is the following:
    //-- We can specify how much information to return
    //-- Instead of bringing back all AppUser and Acitivity data
    //-- we can trim the data and send only the data we need.
    public class CommentDto {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
    }
}