
export class floorsResponse {
  item1?: number;
  item2?: Array<rooms>;
}

export class rooms {
  item1?: string;
  item2?: string;
}

export class updateRoomRequest {
  roomname?: string;
  roomstatus?: string;
  repaircompleted?: boolean;
}

