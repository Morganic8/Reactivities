import React from 'react';
import { Form as FinalForm, Field } from 'react-final-form';
import TextInput from '../../app/common/form/TextInput';
import TextAreaInput from '../../app/common/form/TextAreaInput';
import { combineValidators, isRequired } from 'revalidate';
import { IProfile } from '../../app/models/profile';
import { observer } from 'mobx-react-lite';
import { Button, Form } from 'semantic-ui-react';

interface IProps {
  profile: IProfile;
  updateProfile: (profile: IProfile) => void;
}
const ProfileEditForm: React.FC<IProps> = ({ profile, updateProfile }) => {
  const validate = combineValidators({
    displayName: isRequired({ message: 'A display name is required' }),
  });

  return (
    <FinalForm
      onSubmit={updateProfile}
      validate={validate}
      initialValues={profile}
      render={({ handleSubmit, pristine, invalid, submitting }) => (
        <Form onSubmit={handleSubmit} error>
          <Field
            name="displayName"
            value={profile?.displayName}
            component={TextInput}
          />
          <Field
            name="bio"
            value={profile?.bio}
            component={TextAreaInput}
            rows={3}
          />
          <Button
            disabled={invalid || pristine}
            floated="right"
            positive
            content="Update Profile"
            loading={submitting}
          />
        </Form>
      )}
    />
  );
};

export default observer(ProfileEditForm);
