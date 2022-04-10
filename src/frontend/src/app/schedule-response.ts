import { Lesson } from "./lesson";

export interface ScheduleResponse {
    groupName: string;
    groupId: number;
    lessons: Map<string, Map<string, Lesson[]>>;
    startDate: Date;
    endDate: Date;
}
