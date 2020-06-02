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

#ifndef Unity_iPhone_ACPAnalyticsWrapper_h
#define Unity_iPhone_ACPAnalyticsWrapper_h

extern "C" {
    const char *acp_Analytics_ExtensionVersion();
    void acp_Analytics_RegisterExtension();
    void acp_SendQueuedHits();
    void acp_ClearQueue();
    void acp_GetQueueSize(void (*callback)(long queueSize));
    void acp_GetTrackingIdentifier(void (*callback)(const char *trackingIdentifier));
    void acp_SetVisitorIdentifier(const char *visitorIdentifier);
    void acp_GetVisitorIdentifier(void (*callback)(const char *visitorIdentifier));
}

#endif
