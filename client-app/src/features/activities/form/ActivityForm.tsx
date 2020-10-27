import React, { FormEvent, useContext, useState } from 'react';
import { Button, Form, Segment } from 'semantic-ui-react';
import { IActivity } from '../../../app/models/activity';
import { v4 as uuid } from 'uuid';
import ActivityStore from '../../../app/stores/acvitivtyStore';
import { observer } from 'mobx-react-lite';

interface IProps {
  activity: IActivity;
}

const ActivityForm: React.FC<IProps> = ({ activity: initialFormState }) => {
  const activityStore = useContext(ActivityStore);
  const { createActivity, submitting, cancelFormOpen } = activityStore;
  const initializeForm = () => {
    if (initialFormState) {
      return initialFormState;
    } else {
      return {
        id: '',
        title: '',
        category: '',
        description: '',
        date: '',
        city: '',
        venue: '',
      };
    }
  };

  const [activity, setActivity] = useState<IActivity>(initializeForm);

  const handleInputChange = (
    event: FormEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = event.currentTarget;
    setActivity({ ...activity, [name]: value });
  };

  const handleSubmit = () => {
    if (activity.id.length === 0) {
      let newActivity = {
        ...activity,
        id: uuid(),
      };
      createActivity(newActivity);
    } else {
      activityStore.editActivity(activity);
    }
  };

  return (
    <div>
      <Segment clearing>
        <Form onSubmit={handleSubmit}>
          <Form.Input
            onChange={handleInputChange}
            name="title"
            placeholder="Title"
            value={activity.title}
          />
          <Form.TextArea
            rows={2}
            name="description"
            onChange={handleInputChange}
            placeholder="Description"
            value={activity.description}
          />
          <Form.Input
            name="category"
            onChange={handleInputChange}
            placeholder="Category"
            value={activity.category}
          />
          <Form.Input
            name="date"
            type="datetime-local"
            onChange={handleInputChange}
            placeholder="Date"
            value={activity.date}
          />
          <Form.Input
            name="city"
            placeholder="City"
            value={activity.city}
            onChange={handleInputChange}
          />
          <Form.Input
            name="venue"
            placeholder="Venue"
            value={activity.venue}
            onChange={handleInputChange}
          />
          <Button
            loading={submitting}
            floated="right"
            positive
            type="submit"
            content="Submit"
          />
          <Button
            onClick={cancelFormOpen}
            floated="right"
            type="submit"
            content="Cancel"
          />
        </Form>
      </Segment>
    </div>
  );
};

export default observer(ActivityForm);
