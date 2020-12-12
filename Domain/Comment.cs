using System;

namespace Domain {
    public class Comment {
        public Guid Id { get; set; }
        public string Body { get; set; }
        //need virtual for lazy loading
        public virtual AppUser Author { get; set; }
        //need virtual for lazy loading
        public virtual Activity Activity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}