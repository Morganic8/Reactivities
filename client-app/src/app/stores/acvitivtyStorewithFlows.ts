import { flow, makeAutoObservable, configure, runInAction } from "mobx";
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

  loadActivities = flow(function* (this: ActivityStore){
    this.loadingInitial = true;

    try{
      const activities = yield agent.Activities.list()
      activities.forEach((activity: IActivity) => {
        activity.date = activity.date.split('.')[0];
        //Make a Map of activities
        this.activityRegistry.set(activity.id, activity)
      })
      this.loadingInitial = false;
    } catch(error){
        console.log(error)
        this.loadingInitial = false;
    }
  })

  createActivity = flow(function* (this: ActivityStore, activity: IActivity){
      this.submitting = true;
      try {
        yield agent.Activities.create(activity);
        this.activityRegistry.set(activity.id, activity)
        this.editMode = false;
        this.submitting = false;
      } catch(error) {
        this.submitting = false;
        console.log(error);

      }
  });

  editActivity = flow(function* (this: ActivityStore, activity: IActivity){
    this.submitting = true;
    try {
      yield agent.Activities.update(activity);
      this.activityRegistry.set(activity.id, activity);
      this.selectedActivity = activity;
      this.editMode = false;
      this.submitting = false;
    } catch(error) {

      this.submitting = false;
      console.log(error);

    }
  });

  deleteActivity = flow(function* (this: ActivityStore, event: SyntheticEvent<HTMLButtonElement>, id: string){
    this.submitting = true;
    this.target = event.currentTarget.name;

    try {
      //Remove from DB
      yield agent.Activities.delete(id)  
      //Remove from Local state
      this.activityRegistry.delete(id)
      this.submitting = false;
      this.target = "";
    } catch (error) {
      this.submitting = false;
      this.target = "";
      console.log(error);
    }
    
  })

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