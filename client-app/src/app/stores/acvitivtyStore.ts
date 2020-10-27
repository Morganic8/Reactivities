import { makeAutoObservable, configure, runInAction, action } from "mobx";
import { createContext, SyntheticEvent } from "react";
import agent from "../api/agent";
import { IActivity } from "../models/activity";

configure({enforceActions: "always"});

class ActivityStore {


  //observables
   activityRegistry = new Map();
   activities: IActivity[] = [];
   selectedActivity: IActivity | undefined;
   editMode = false
   submitting = false;
   target = "";

   //computed
   get activtiesByDate(){
     return Array.from(this.activityRegistry.values()).sort((a,b) => Date.parse(a.date) - Date.parse(b.date))
   }

  loadingInitial = false;

  constructor(){
    makeAutoObservable(this);
    
  }

  loadActivities = async () => {
    this.loadingInitial = true;
    try {
      const activities = await agent.Activities.list();
      runInAction(() => {
        activities.forEach(activity => {
          activity.date = activity.date.split('.')[0];
          this.activityRegistry.set(activity.id, activity);
        });
        this.loadingInitial = false;
      })
    } catch (error) {
      runInAction(() => {
        this.loadingInitial = false;
      })
      console.log(error);
    }
  }

  createActivity = async (activity: IActivity) => {
    this.submitting = true;
    try {
      await agent.Activities.create(activity);
      runInAction(() => {
        this.activityRegistry.set(activity.id, activity);
        this.editMode = false;
        this.submitting = false;
      })
    } catch (error) {
      runInAction(() => {
        this.submitting = false;
      })
      console.log(error);
    }
  }

  editActivity = async (activity: IActivity) => {
    this.submitting = true;
    try {
      await agent.Activities.update(activity);
      runInAction(() => {
        this.activityRegistry.set(activity.id, activity);
        this.selectedActivity = activity;
        this.editMode = false;
        this.submitting = false;
      })
    } catch (error) {
      runInAction(() => {
        this.submitting = false;
      })
      console.log(error);
    }
  }

  deleteActivity = async (event: SyntheticEvent<HTMLButtonElement>, id: string) => {
    this.submitting = true;
    this.target = event.currentTarget.name;

    try {
      //Remove from DB
      await agent.Activities.delete(id)  
      //Remove from Local state
      runInAction(() => {
        this.activityRegistry.delete(id)
        this.submitting = false;
        this.target = "";
      })

    } catch (error) {
      runInAction(() => {
        this.submitting = false;
        this.target = "";
      })
      console.log(error);
    }
  };

  openEditForm = (id: string) => {
    this.selectedActivity = this.activityRegistry.get(id);
    this.editMode = true;
  }

  openCreateForm = () => {
    this.editMode = true;
    this.selectedActivity = undefined;
  }

  cancelSelectedActivity = () => {
    this.selectedActivity = undefined;
  }

  cancelFormOpen = () => {
    this.editMode = false;
  }

  selectActivity = (id: string) => {
    this.selectedActivity = this.activityRegistry.get(id);
    this.editMode = false;
  }
}

export default createContext(new ActivityStore())