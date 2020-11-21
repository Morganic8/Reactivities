import { action, makeAutoObservable, observable, reaction } from 'mobx';
import { RootStore } from './rootStore';

export default class CommonStore {
  //bring in root store
  rootStore: RootStore;

  @observable token: string | null = window.localStorage.getItem('jwt');
  @observable appLoaded = false;

  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;
    makeAutoObservable(this);

    reaction(
      //Expression - specifity what you want to react on => this.token
      () => this.token,
      (token) => {
        //if token exists set jwt, remove if not
        if (token) {
          window.localStorage.setItem('jwt', token);
        } else {
          window.localStorage.removeItem('jwt');
        }
      }
    );
  }

  @action setToken = (token: string | null) => {
    this.token = token;
  };

  @action setAppLoaded = () => {
    this.appLoaded = true;
  };
}
