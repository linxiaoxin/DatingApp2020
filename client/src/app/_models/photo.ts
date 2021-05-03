
export interface Photo {
    id: number;
    url: string;
    isMain: boolean;
    isApproved: boolean;
    moderateDate?: Date;
    userName: string;
    knownAs: string;
}
