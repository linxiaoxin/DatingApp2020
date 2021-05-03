import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getPhotoToModeration();
  }

  getPhotoToModeration(){
    this.adminService.getPhotoToModerate().subscribe(photos => {
        this.photos = photos;
    });
  }

  approve(photoId){
    this.adminService.approvePhoto(photoId).subscribe(()=>{
      this.photos = this.photos.filter(p => p.id != photoId);
    });
  }

  reject(photoId){
    this.adminService.rejectPhoto(photoId).subscribe(()=>{
      this.photos = this.photos.filter(p => p.id != photoId);
    });
  }
}
