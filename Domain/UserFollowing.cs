namespace Domain {

    //Join entity - This allows for Followers and Following AppUsers
    public class UserFollowing {
        public string  ObserverId { get; set; }

        //Navigation Property
        public virtual AppUser Observer { get; set; }

        public string TargetId { get; set; }

        //Navigation Property
        public virtual AppUser Target { get; set; }
    }
}