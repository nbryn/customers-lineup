import {DTO} from './General';

export interface TimeSlotDTO extends DTO {
    date: string;
    capacity: string;
    start: string;
    end: string;
}