import {DTO} from './General';

export interface BookingDTO extends DTO {
    timeSlotId: number;
    business: string;
    userMail: string;
    date: string;
    capacity: string;
    interval: string;
}