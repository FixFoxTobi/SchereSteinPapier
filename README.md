Haha lol hier müsste die Doku stehen!!!!1

---blind kopie

# Multimodale Mensch-Maschine-Interaktion

Bearbeiter: Niebisch, Markus, 54795

## Quellen

### Visuelle und auditive Verzögerungen: Einfluss auf Leistung bei visuomotorischen Aufgaben

Das Paper von Dix et al. untersucht die Auswirkungen von Verzögerungen bei visueller und auditiver Rückmeldung auf die Leistung und Bewegungsstrategie bei einer visuomotorischen Aufgabe [Dix et al. 2023]. In einem Experiment spielten Teilnehmer das Spiel "Hot Wire", um die Effekte von vier verschiedenen Feedback-Bedingungen zu testen: keine Verzögerung, nur visuelle Verzögerung, nur auditive Verzögerung und audiovisuelle Verzögerung (200 ms). Eine visuelle Verzögerung führte zu verlangsamten Bewegungen und verringerter Präzision, was im Einklang mit früheren Studien steht. Entgegen anderen Studien verbesserte jedoch eine auditive Verzögerung die Präzision; das Paper diskutiert mögliche Erklärungen für diesen Befund. Die Kombination von visuellen und auditiven Verzögerungen zeigte keine signifikanten zusätzlichen Effekte im Vergleich zu den einzelnen Verzögerungsbedingungen. Die Studie hebt hervor, dass die zeitliche Kongruenz von multisensorischen Feedback sowie mögliche kompensatorische Mechanismen berücksichtigt werden sollten, um die Mensch-Maschine-Interaktion in Bezug auf Latenzen zu verbessern.
 

![Abbildung Versuchsaufbau "Wire Loop Game" und Beispiel Spiel](img/wire_loop_setup.png)

### Einfluss von Verzögerungen in visueller und haptischer Rückmeldung bei einer Tapping-Aufgabe

Im Paper von Jay und Hubbold wird die Auswirkungen von verzögerter visueller und haptischer Rückmeldung auf die Leistung in einer sich wiederholenden Tapping-Aufgabe untersucht [Jay & Hubbold 2005]. In einem Experiment tippten die Teilnehmer abwechselnd zwischen zwei Zielen, wobei verschiedene Verzögerungen (0, 25, 50, 75 und 150 ms) entweder nur auf visuelles Feedback, nur auf haptisches Feedback oder auf beide Feedback-Typen angewendet wurden. Die Ergebnisse zeigen, dass eine visuelle Verzögerung die Bewegungsgeschwindigkeit signifikant beeinträchtigt und zu einer höheren Fehlerquote sowie zu einem subjektiv als schwieriger empfundenen Task führt. Eine Verzögerung im haptischen Feedback hingegen beeinflusst die Leistung erst bei einer hohen Latenz (etwa 200 ms) und führt hauptsächlich zu längeren Bewegungszeiten, jedoch ohne die Fehlerquote oder wahrgenommene Schwierigkeit signifikant zu erhöhen. Die Studie schlussfolgert, dass visuelle Verzögerungen deutlich störender wirken als haptische, was für die Gestaltung von verteilten virtuellen Umgebungen (DVEs) und Mensch-Maschine-Interaktionen relevant ist.


![Abbildung Versuchsaufbau Visual & Haptik Delay](img/modularitaeten_visual_haptik.png)

## Erweiterung und Variation

| | Dix et al. 2023 | Jay & Hubbold 2005 |
| --- | --- | --- |
| Bildschirm | Bildübertragung | Bildübertragung |
| Kopfhörer | Fehlersignal senden | *Annäherungssignal übertragen* |
| HaptikMaster | *Vibrationen im Fehlerfall erzeugen* | Objektberührung simulieren |

![Abbildung des CARE-Modells](img/care-model.png)

Annahme: Da es sich um einen minimale Zeitversätze handelt, wird trotz der Verzögerung von einer parallelen Ausführung der Modalitäten im System ausgegangen.

## CROW-Framework

| Dimension | Description |
|---|---|
| Character | Ein ältere Uhrenbauer könnte als Meister seines Fachs beschrieben werden, der durch jahrelange Erfahrung exzellentes Wissen über Feinmechanik gesammelt hat. Aufgrund seines Alters ist er jedoch körperlich eingeschränkt, was zu Zittrigkeit und verminderten Feinmotorikfähigkeiten führt. Sein hohes Verantwortungsbewusstsein und sein starker Wunsch, weiterhin produktiv zu arbeiten, geben ihm die Motivation, neue Technologien zu erlernen und anzuwenden. |
| Relationship | Die anfängliche Skepsis gegenüber einer Maschinensteuerung wandelt sich im Laufe der Nutzung zu einem positiven Gefühl der Unterstützung. Ein Roboterarm dient als zuverlässiger Partner, der die körperlichen Einschränkungen des Uhrenbauers ausgleicht. Die Beziehung könnte als „ergänzende Partnerschaft“ beschrieben werden, bei der die Maschine seine physische Präzision verstärkt, während er die Fachkenntnis und das Gespür für das Detail einbringt. |
| Objective | Das Ziel des Uhrenbauers ist es, die Qualität und Präzision seiner Arbeit aufrechtzuerhalten. Die Maschine ermöglicht es ihm, trotz seiner motorischen Einschränkungen weiterhin in seinem geliebten Beruf tätig zu sein. Langfristig strebt er nach einem Gleichgewicht zwischen der Erhaltung seiner Fähigkeiten und der Anpassung an moderne Technologien, um nachhaltig beschäftigt zu bleiben. |
| Where | Das Arbeitsumfeld befindet sich in einer ruhigen, gut beleuchteten Werkstatt, die speziell für Präzisionsarbeiten ausgestattet ist. Der Roboterarm ist fest installiert, und der Arbeitsplatz bietet die nötigen Bedingungen für konzentriertes Arbeiten. Die multimodalen Rückmeldungen (visuell, auditiv und haptisch) unterstützen den Uhrenbauer dabei, den Roboterarm mit hoher Präzision zu steuern und die nötige Feinheit bei der Arbeit zu gewährleisten. |

## Storyboard

![Storyboard](img/storyboard.png)

## Literaturverzeichnis

[Dix et al. 2023] Annika Dix, Clarissa Sabrina Arlinghaus, A. Marie Harkin, and Sebastian Pannasch. 2023. The Role of Audiovisual Feedback Delays and Bimodal Congruency for Visuomotor Performance in Human-Machine Interaction. In Proceedings of the 25th International Conference on Multimodal Interaction (ICMI '23). Association for Computing Machinery, New York, NY, USA, 555–563. https://doi.org/10.1145/3577190.3614111

[Jay & Hubbold 2005] Caroline Jay, and Roger Hubbold. 2005. Delayed visual and haptic feedback in a reciprocal tapping task. First Joint Eurohaptics Conference and Symposium on Haptic Interfaces for Virtual Environment and Teleoperator Systems. World Haptics Conference (WHC 2005), Pisa, Italy, 655-656. https://doi.org/10.1109/WHC.2005.29

[1] Dryad Education (2023): Buzz Wire Game [online] URL: https://www.dryadeducation.co.uk/buzz-wire-game [JPG-Datei] [Stand 15.10.2024]

[2] Research Gate (2005): Delayed Visual and Haptic Feedback in a Reciprocal Tapping Task [online] URL: https://www.researchgate.net/figure/Experimental-set-up_fig1_221012159 [PNG-Datei] [Stand 29.10.2024]

