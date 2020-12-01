import React from 'react';
import { Popup, Item, List } from 'semantic-ui-react';
import { IAttendee } from '../../../app/models/activity';
interface IProps {
  attendees: IAttendee[];
}

const ActivityListItemAttendees: React.FC<IProps> = ({ attendees }) => {
  return (
    <List horizontal>
      {attendees.map((attendee) => (
        <List.Item key={attendee.username}>
          <Popup
            header={attendee.displayName}
            trigger={
              <Item.Image
                size="mini"
                circular
                src={attendee.image || '/assets/user.png'}
              />
            }
          />
        </List.Item>
      ))}
    </List>
  );
};

export default ActivityListItemAttendees;
