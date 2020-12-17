import { observer } from 'mobx-react-lite';
import React, { useContext, useEffect } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { Grid, GridColumn } from 'semantic-ui-react';
import LoadingComponent from '../../app/layout/LoadingComponent';
import { RootStoreContext } from '../../app/stores/rootStore';
import ProfileContent from './ProfileContent';
import ProfileHeader from './ProfileHeader';

//username comes from url
interface RouteParams {
  username: string;
}

//RouteCompProps grants access to the username in url
interface IProps extends RouteComponentProps<RouteParams> {}

const ProfilePage: React.FC<IProps> = ({ match }) => {
  const rootStore = useContext(RootStoreContext);
  const {
    loadingProfile,
    profile,
    loadProfile,
    follow,
    unfollow,
    isCurrentUser,
    loading,
    setActiveTab,
  } = rootStore.profileStore;

  useEffect(() => {
    //inject url param username here
    loadProfile(match.params.username);
  }, [loadProfile, match]);

  if (loadingProfile) return <LoadingComponent content="Loading profile..." />;

  return (
    <Grid>
      <GridColumn width={16}>
        <ProfileHeader
          profile={profile!}
          isCurrentUser={isCurrentUser}
          loading={loading}
          follow={follow}
          unfollow={unfollow}
        />
        <ProfileContent setActiveTab={setActiveTab} />
      </GridColumn>
    </Grid>
  );
};

export default observer(ProfilePage);
