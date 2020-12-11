import React, { useContext, useState } from 'react';
import { Button, Grid, Header, Tab } from 'semantic-ui-react';
import { RootStoreContext } from '../../app/stores/rootStore';
import ProfileEditForm from './ProfileEditForm';

const ProfileAbout = () => {
  const rootStore = useContext(RootStoreContext);
  const { isCurrentUser, profile, editProfile } = rootStore.profileStore;
  const [editProfileMode, setEditProfileMode] = useState(false);

  const handleFinalFormSubmit = (values: any) => {
    const { ...profile } = values;
    editProfile(profile).then(() => setEditProfileMode(!editProfileMode));
  };
  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16}>
          <Header
            floated="left"
            icon="user"
            content={`About ${profile?.displayName}`}
          />
          {isCurrentUser && (
            <Button
              basic
              floated="right"
              content={editProfileMode ? 'Cancel' : 'Edit Profile'}
              onClick={() => setEditProfileMode(!editProfileMode)}
            />
          )}
        </Grid.Column>
        <Grid.Column width={16}>
          {editProfileMode ? (
            <ProfileEditForm
              profile={profile!}
              updateProfile={handleFinalFormSubmit}
            />
          ) : (
            <span>{profile?.bio}</span>
          )}
        </Grid.Column>
      </Grid>
    </Tab.Pane>
  );
};

export default ProfileAbout;
