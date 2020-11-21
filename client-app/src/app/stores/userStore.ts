import { action, makeAutoObservable, runInAction } from 'mobx';
import { history } from '../..';
import agent from '../api/agent';
import { IUser, IUserFormValues } from '../models/user';
import { RootStore } from './rootStore';

export default class UserStore {
  //observable
  user: IUser | null = null;
  rootStore: RootStore;

  constructor(rootStore: RootStore) {
    makeAutoObservable(this);
    this.rootStore = rootStore;
  }

  //computed
  get isLoggedIn() {
    return !!this.user;
  }

  //action
  login = async (values: IUserFormValues) => {
    try {
      const user = await agent.User.login(values);
      runInAction(() => {
        this.user = user;
        this.rootStore.commonStore.setToken(user.token);
        this.rootStore.modalStore.closeModal();
      });

      history.push('/activities');
    } catch (error) {
      throw error;
    }
  };

  @action register = async (values: IUserFormValues) => {
    try {
      const user = await agent.User.register(values);
      runInAction(() => {
        this.rootStore.commonStore.setToken(user.token);
        this.rootStore.modalStore.closeModal();
      });
      history.push('/activities');
    } catch (error) {
      throw error;
    }
  };

  @action getUser = async () => {
    try {
      const user = await agent.User.current();
      runInAction(() => {
        this.user = user;
      });
    } catch (error) {
      console.log(error);
    }
  };

  @action logout = () => {
    runInAction(() => {
      this.rootStore.commonStore.setToken(null);
      this.user = null;
    });
    history.push('/');
  };
}
