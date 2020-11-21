import { action, makeAutoObservable, observable } from 'mobx';
import { RootStore } from './rootStore';

export default class ModalStore {
  rootStore: RootStore;
  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;
    makeAutoObservable(this);
  }

  //Observables that are objects need to be specficied as shallow
  //otherwise it does a deep comparison - We only want the shallow properties to compare
  @observable.shallow modal = {
    open: false,
    body: null,
  };

  @action openModal = (content: any) => {
    this.modal.open = true;
    this.modal.body = content;
  };

  @action closeModal = () => {
    this.modal.open = false;
    this.modal.body = null;
  };
}
