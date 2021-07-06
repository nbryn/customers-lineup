import {DTO} from '../../app/General';

export interface BookingDTO extends DTO {
    timeSlotId: string;
    address: string;
    business: string;
    longitude: number;
    latitude: number;
    userMail: string;
    date: string;
    capacity: string;
    interval: string;
}