import { Component, Input, OnChanges, OnDestroy, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';

declare const JitsiMeetExternalAPI: any;

@Component({
  selector: 'app-jitsi',
  standalone: true,
  imports: [CommonModule],
  template: `<div #jitsiContainer class="jitsi-container"></div>`,
  styles: [`
    :host { display: flex; flex-direction: column; flex: 1; min-height: 0; }
    .jitsi-container { flex: 1; min-height: 0; }
    .jitsi-container iframe { width: 100%; height: 100%; border: none; }
  `]
})
export class JitsiComponent implements AfterViewInit, OnChanges, OnDestroy {
  @Input() roomCode: string = '';
  @Input() displayName: string = '';

  @ViewChild('jitsiContainer') containerRef!: ElementRef<HTMLDivElement>;

  private api: any = null;

  ngAfterViewInit(): void {
    this.initJitsi();
  }

  ngOnChanges(): void {
    if (this.containerRef) {
      this.dispose();
      this.initJitsi();
    }
  }

  ngOnDestroy(): void {
    this.dispose();
  }

  private initJitsi(): void {
    if (!this.roomCode || !this.displayName) return;
    if (typeof JitsiMeetExternalAPI === 'undefined') return;

    const roomName = 'yatzy-' + this.roomCode.toLowerCase();

    this.api = new JitsiMeetExternalAPI('meet.jit.si', {
      roomName,
      parentNode: this.containerRef.nativeElement,
      userInfo: { displayName: this.displayName },
      configOverwrite: {
        startWithAudioMuted: false,
        startWithVideoMuted: false,
        disableDeepLinking: true,
        prejoinPageEnabled: false,
      },
      interfaceConfigOverwrite: {
        TOOLBAR_BUTTONS: ['microphone', 'camera', 'hangup', 'tileview'],
        SHOW_JITSI_WATERMARK: false,
        SHOW_WATERMARK_FOR_GUESTS: false,
        SHOW_BRAND_WATERMARK: false,
        DEFAULT_REMOTE_DISPLAY_NAME: 'Spiller',
        HIDE_INVITE_MORE_HEADER: true,
      },
    });
  }

  private dispose(): void {
    if (this.api) {
      try { this.api.dispose(); } catch { /* ignore */ }
      this.api = null;
    }
  }
}
