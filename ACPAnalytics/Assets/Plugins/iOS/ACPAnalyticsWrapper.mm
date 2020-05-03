/*
Copyright 2020 Adobe. All rights reserved.
This file is licensed to you under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License. You may obtain a copy
of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under
the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
OF ANY KIND, either express or implied. See the License for the specific language
governing permissions and limitations under the License.
*/

#import "ACPAnalyticsWrapper.h"
#import "ACPAnalytics.h"

const char *acp_Analytics_ExtensionVersion() {
   return [[ACPAnalytics extensionVersion] cStringUsingEncoding:NSUTF8StringEncoding];
}

void acp_Analytics_RegisterExtension() {
    [ACPAnalytics registerExtension];
}

void acp_SendQueuedHits() {
    [ACPAnalytics sendQueuedHits];
}

void acp_ClearQueue() {
    [ACPAnalytics clearQueue];
}

void acp_GetQueueSize(void (*callback)(long queueSize)){
    [ACPAnalytics getQueueSize: ^(NSUInteger queueSize) {
        if (callback != nil) {
            callback((long)queueSize);
        }
    }];
}

void acp_GetTrackingIdentifier(void (*callback)(const char *trackingIdentifier)) {
    [ACPAnalytics getTrackingIdentifier:^(NSString * _Nullable trackingIdentifier) {
        if (callback != nil) {
            callback([trackingIdentifier cStringUsingEncoding:NSUTF8StringEncoding]);
        }
    }];
}

void acp_SetVisitorIdentifier(const char *visitorIdentifier) {
    NSString *visitorIdentifierString = [NSString stringWithCString:visitorIdentifier encoding:NSUTF8StringEncoding];
    if (!visitorIdentifierString.length) {
        NSLog(@"Visitor Identifier string is empty or nil");
    } else {
        [ACPAnalytics setVisitorIdentifier:visitorIdentifierString];
    }
}

void acp_GetVisitorIdentifier(void (*callback)(const char *visitorIdentifier)) {
    [ACPAnalytics getVisitorIdentifier:^(NSString * _Nullable visitorIdentifier) {
        if (callback != nil) {
            callback([visitorIdentifier cStringUsingEncoding:NSUTF8StringEncoding]);
        }
    }];
}
