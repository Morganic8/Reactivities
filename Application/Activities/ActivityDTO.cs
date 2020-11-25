using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Activities {
    public class ActivityDTO {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }

        //return a list of Attendees instead of UserActivities
        //Tell Automapper to look for UserActivities profile
        //JsonPropertyName - gives us the custom label for the json

        [JsonPropertyName("attendees")]
        public ICollection<AttendeeDTO> UserActivities { get; set; }

    }
}