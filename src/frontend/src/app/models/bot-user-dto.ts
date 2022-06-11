export interface BotUserDto {
    id: number;
    weatherCity: string;
    narfuGroup: number;
    
    isAdmin: boolean;
    hasWeatherSubscription: boolean;
    hasScheduleSubscription: boolean;
}
