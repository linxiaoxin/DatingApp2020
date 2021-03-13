export interface Message {
  id: number;
  senderId: number;
  senderUserName: string;
  senderPhotoUrl: string;
  recipientId: number;
  recipientUserName: string;
  recipientPhotoUrl: string;
  content: string;
  dateSent: Date;
  dateRead?: Date;
}