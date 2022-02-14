
import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { floorsResponse, updateRoomRequest } from '././Home.Model'


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {

  floorsSummary: Array<floorsResponse> = [];
  nearestAvailableRoomResult: string = "";
  reservedRoomResult: string = "";
  checkoutRoomResult: string = "";
  repairRoomResult: string = "";
  cleanedRoomResult: string = "";
  repairCompletedRoomResult: string = "";

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

  }

  ngOnInit() {
    this.loadRooomsLayout();
  }

  loadRooomsLayout() {
    this.http.get<floorsResponse[]>(this.baseUrl + 'hotelbinary/rooms-layout').subscribe(result => {
      if (result != null) {
        this.floorsSummary = result;
        console.log(this.floorsSummary);
      }
    }, error => console.error(error));
  }

  findNearestAvailableRoom() {
    this.http.get(this.baseUrl + 'hotelbinary/search-nearest', { responseType: 'text' }).subscribe(result => {
      if (result != null) {
        this.nearestAvailableRoomResult = result;
        console.log(result);
      }
    }, error => console.error(error));
  }

  searchNearestRoom() {
    this.findNearestAvailableRoom();
  }

  async reserveRoom() {
    let requestObj = new updateRoomRequest();
    requestObj.roomname = (<HTMLInputElement>document.getElementById("roomInputReserve")).value.toUpperCase();
    requestObj.roomstatus = "O";
    if (requestObj.roomname && requestObj.roomname != "") {
      this.reservedRoomResult =  await this.updateRoom(requestObj);
    } 
  }
  async checkoutRoom() {
    let requestObj = new updateRoomRequest();
    requestObj.roomname = (<HTMLInputElement>document.getElementById("roomInputCheckout")).value.toUpperCase();
    requestObj.roomstatus = "V";
    if (requestObj.roomname && requestObj.roomname != "") {
      this.checkoutRoomResult = await this.updateRoom(requestObj);
    }
  }
  async repairRoom() {
    let requestObj = new updateRoomRequest();
    requestObj.roomname = (<HTMLInputElement>document.getElementById("roomInputRepair")).value.toUpperCase();
    requestObj.roomstatus = "R";
    if (requestObj.roomname && requestObj.roomname != "") {
      this.repairRoomResult = await this.updateRoom(requestObj);
    }
  }
  async repairCompletedRoom() {
    let requestObj = new updateRoomRequest();
    requestObj.roomname = (<HTMLInputElement>document.getElementById("roomInputRepairCompleted")).value.toUpperCase();
    requestObj.roomstatus = "V";
    requestObj.repaircompleted = true;
    if (requestObj.roomname && requestObj.roomname != "") {
      this.repairCompletedRoomResult = await this.updateRoom(requestObj);
    }
  }

  async cleanedRoom() {
    let requestObj = new updateRoomRequest();
    requestObj.roomname = (<HTMLInputElement>document.getElementById("roomInputCleaned")).value.toUpperCase();
    requestObj.roomstatus = "A";
    if (requestObj.roomname && requestObj.roomname != "") {
      this.cleanedRoomResult = await this.updateRoom(requestObj);
    }
  }

  //updateRoom(updateRoomRequest: updateRoomRequest) {
  //  let postResult = "";
  //  this.http.post(this.baseUrl + 'weatherforecast/update-room', updateRoomRequest, { responseType: 'text' }).subscribe(result => {
  //    if (result != null) {
  //      postResult = result;
  //      this.loadRooomsLayout();
  //    }
  //  }, error => console.error(error));
  //  return postResult;
  //}

  async updateRoom(updateRoomRequest: updateRoomRequest) {
    let postResult: any;
    await this.http.post(this.baseUrl + 'hotelbinary/update-room', updateRoomRequest, { responseType: 'text' }).toPromise()
      .catch((error: any) => console.log("Update Failed"))
      .then(result => {
        postResult = result;
        this.loadRooomsLayout();
      });
    return postResult;
  }
}
