import { Photo } from "./photo";

export interface User {
  // Convention: anuglar use lowercase for attributes
  id: number;
  username: string;
  knowAs: string;
  knownAs: string;  // need to add knownAs to register, because knownAs is a attribute in back-end
  age: number;
  gender: string;
  created: Date;
  // lastActive: Date;  // If use Date, when production, it would got Argument of type 'Date' is not assignable to parameter of type 'string'
  lastActive: any;   
  photoUrl: string;
  city: string;
  country: string;

  // Need to include properties for UserForDetailedDto in Backend
  //  ? => set as optional
  interests?: string;
  introduction?: string;
  lookingFor?: string;

  // Need to create Photo Interface, because want to add a separate one to store all of properties
  photos?: Photo[];
}
